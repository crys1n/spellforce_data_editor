﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapLakeInspector : SpellforceDataEditor.SFMap.map_controls.MapInspector
    {
        SFMapLake selected_lake = null;

        public MapLakeInspector()
        {
            InitializeComponent();
        }

        public override void OnSelect(object o)
        {
            if (o == null)
                Enabled = false;
            else
            {
                if (o.GetType() == typeof(SFMapLake))
                {
                    selected_lake = (SFMapLake)o;
                    Enabled = true;
                    if (selected_lake.type == 0)
                        LakeTypeWater.Checked = true;
                    else if (selected_lake.type == 1)
                        LakeTypeSwamp.Checked = true;
                    else if (selected_lake.type == 2)
                        LakeTypeLava.Checked = true;
                    else if (selected_lake.type == 3)
                        LakeTypeIce.Checked = true;
                    else
                        throw new Exception("MapLakeInspector.OnSelect(): Invalid lake type!");
                    SelectedLakeDepth.Text = selected_lake.z_diff.ToString();
                    SelectedLakeID.Text = selected_lake.GetObjectName();
                }
                else
                    Enabled = false;
            }
            if (!Enabled)
                selected_lake = null;
        }

        private void LakeTypeWater_CheckedChanged(object sender, EventArgs e)
        {
            selected_lake.type = 0;
            map.lake_manager.UpdateLake(selected_lake);
            MainForm.mapedittool.update_render = true;
        }

        private void LakeTypeSwamp_CheckedChanged(object sender, EventArgs e)
        {
            selected_lake.type = 1;
            map.lake_manager.UpdateLake(selected_lake);
            MainForm.mapedittool.update_render = true;
        }

        private void LakeTypeLava_CheckedChanged(object sender, EventArgs e)
        {
            selected_lake.type = 2;
            map.lake_manager.UpdateLake(selected_lake);
            MainForm.mapedittool.update_render = true;
        }

        private void LakeTypeIce_CheckedChanged(object sender, EventArgs e)
        {
            selected_lake.type = 3;
            map.lake_manager.UpdateLake(selected_lake);
            MainForm.mapedittool.update_render = true;
        }
    }
}
