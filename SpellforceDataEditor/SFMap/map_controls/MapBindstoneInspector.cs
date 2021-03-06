﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapBindstoneInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        bool move_camera_on_select = false;
        bool bindstone_selected_from_list = true;

        public MapBindstoneInspector()
        {
            InitializeComponent();
        }

        private void MapBindstoneInspector_Load(object sender, EventArgs e)
        {
            ReloadList();
            ResizeList();
        }

        private void ReloadList()
        {
            ListBindstones.Items.Clear();
            foreach (SFMapInteractiveObject io in map.int_object_manager.int_objects)
                if (io.game_id == 769)
                    ListBindstones.Items.Add(GetBindstoneString(io));
        }

        // returned value >= argument value, or -1
        private int GetIOBindstoneIndex(int index)
        {
            int found_bindstones = 0;
            for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
            {
                SFMapInteractiveObject io = map.int_object_manager.int_objects[i];
                if (io.game_id == 769)
                {
                    if (found_bindstones == index)
                        return i;
                    found_bindstones += 1;
                }
            }
            return -1;
        }

        private int GetBindstoneIndex(SFMapInteractiveObject o)
        {
            int found_bindstones = 0;
            for (int i = 0; i < map.int_object_manager.int_objects.Count; i++)
            {
                SFMapInteractiveObject io = map.int_object_manager.int_objects[i];
                if (io.game_id == 769)
                {
                    if (o == io)
                        return found_bindstones;
                    found_bindstones += 1;
                }
            }
            return -1;
        }

        private int GetPlayerIndexByBindstone(SFMapInteractiveObject o)
        {
            return map.metadata.FindPlayerBySpawnPos(o.grid_position);
        }

        private string GetBindstoneString(SFMapInteractiveObject io)
        {
            int player = GetPlayerIndexByBindstone(io);
            if (player == -1)
                return "Bindstone at " + io.grid_position.ToString();
            else
            {
                if(map.metadata.spawns[player].text_id == 0)
                    return "Bindstone at " + io.grid_position.ToString();
                else
                {
                    SFCFF.SFCategoryElement elem = SFCFF.SFCategoryManager.FindElementText(
                        map.metadata.spawns[player].text_id, Settings.LanguageID);
                    if(elem == null)
                        return "Bindstone at " + io.grid_position.ToString();
                    string ret = Utility.CleanString(elem.variants[4]);
                    return ret + " " + io.grid_position.ToString();
                }
            }
        }

        private void ShowList()
        {
            if (ButtonResizeList.Text == "-")
                return;

            ResizeList();

            ButtonResizeList.Text = "-";
        }

        private void ResizeList()
        {
            PanelBindstonesList.Height = this.Height - PanelBindstonesList.Location.Y - 3;
            ListBindstones.Height = PanelBindstonesList.Height - 75;
        }

        public void RemoveBindstone(int index)
        {
            if (ListBindstones.SelectedIndex == index)
                PanelProperties.Enabled = false;
            ListBindstones.Items.RemoveAt(index);
        }

        public void LoadNextBindstone()
        {
            int new_bindstone = GetIOBindstoneIndex(ListBindstones.Items.Count);
            if (new_bindstone == -1)
                return;
            SFMapInteractiveObject io = map.int_object_manager.int_objects[new_bindstone];
            ListBindstones.Items.Add(GetBindstoneString(io));
        }

        private void HideList()
        {
            if (ButtonResizeList.Text == "+")
                return;

            PanelBindstonesList.Height = 30;

            ButtonResizeList.Text = "+";
        }

        public override void OnSelect(object o)
        {
            move_camera_on_select = false;
            bindstone_selected_from_list = false;

            if (o == null)
            {
                map.selection_helper.CancelSelection();
                PanelProperties.Enabled = false;
            }
            else
                ListBindstones.SelectedIndex = GetBindstoneIndex((SFMapInteractiveObject)o);
        }

        private void ListBindstones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == -1)
                return;

            PanelProperties.Enabled = true;
            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[GetIOBindstoneIndex(ListBindstones.SelectedIndex)];
            int player = GetPlayerIndexByBindstone(bindstone);
            if (player == -1)
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap,
                    "MapBindstoneInspector.ListBindstones_SelectedIndexChanged(): Can't find player at position "
                    + bindstone.grid_position.ToString());
            else
            {
                TextID.Text = map.metadata.spawns[player].text_id.ToString();
                Unknown.Text = map.metadata.spawns[player].unknown.ToString();
            }
            PosX.Text = bindstone.grid_position.x.ToString();
            PosY.Text = bindstone.grid_position.y.ToString();
            AngleTrackbar.Value = bindstone.angle;
            // angle, angletrackbar

            map.selection_helper.SelectInteractiveObject(bindstone);
            if ((move_camera_on_select) || (bindstone_selected_from_list))
                MainForm.mapedittool.SetCameraViewPoint(bindstone.grid_position);
            move_camera_on_select = false;
            bindstone_selected_from_list = true;
        }

        private void ButtonResizeList_Click(object sender, EventArgs e)
        {
            if (ButtonResizeList.Text == "-")
                HideList();
            else
                ShowList();
        }

        private void Angle_Validated(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == -1)
                return;

            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[GetIOBindstoneIndex(ListBindstones.SelectedIndex)];

            int v = Utility.TryParseUInt16(Angle.Text, (ushort)bindstone.angle);
            AngleTrackbar.Value = (v >= 0 ? (v <= 359 ? v : 359) : 0);
        }

        private void AngleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == -1)
                return;

            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[GetIOBindstoneIndex(ListBindstones.SelectedIndex)];
            Angle.Text = AngleTrackbar.Value.ToString();
            bindstone.angle = AngleTrackbar.Value;
            map.RotateInteractiveObject(GetIOBindstoneIndex(ListBindstones.SelectedIndex), bindstone.angle);

            MainForm.mapedittool.update_render = true;
        }

        private void TextID_Validated(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == -1)
                return;

            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[GetIOBindstoneIndex(ListBindstones.SelectedIndex)];
            int player = GetPlayerIndexByBindstone(bindstone);
            if (player == -1)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap, 
                    "MapBindstoneInspector.TextID_Validated(): Can't find player at position " 
                    + bindstone.grid_position.ToString());
                return;
            }

            map.metadata.spawns[player].text_id = Utility.TryParseUInt16(TextID.Text, map.metadata.spawns[player].text_id);
            ListBindstones.Items[ListBindstones.SelectedIndex] = GetBindstoneString(bindstone);
        }

        private void Unknown_Validated(object sender, EventArgs e)
        {
            if (ListBindstones.SelectedIndex == -1)
                return;

            SFMapInteractiveObject bindstone = map.int_object_manager.int_objects[GetIOBindstoneIndex(ListBindstones.SelectedIndex)];
            int player = GetPlayerIndexByBindstone(bindstone);
            if (player == -1)
            {
                LogUtils.Log.Warning(LogUtils.LogSource.SFMap,
                    "MapBindstoneInspector.Unknown_Validated(): Can't find player at position "
                    + bindstone.grid_position.ToString());
                return;
            }

            map.metadata.spawns[player].unknown = Utility.TryParseInt16(TextID.Text, map.metadata.spawns[player].unknown);
        }

        private void TextID_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.data == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                int elem_id = Utility.TryParseUInt16(TextID.Text);
                int real_elem_id = SFCFF.SFCategoryManager.gamedata[14].GetElementIndex(elem_id);
                if (real_elem_id != -1)
                    MainForm.data.Tracer_StepForward(14, real_elem_id);
            }
        }
    }
}
