﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_dialog
{
    public partial class MapVisibilitySettings : Form
    {
        public SFMap map;
        bool ready = false;

        public MapVisibilitySettings()
        {
            InitializeComponent();
            checkBox1.Checked = Settings.UnitsVisible;
            checkBox2.Checked = Settings.BuildingsVisible;
            checkBox3.Checked = Settings.ObjectsVisible;
            checkBox4.Checked = Settings.DecorationsVisible;
            checkBox5.Checked = Settings.LakesVisible;
            checkBox6.Checked = Settings.VisualizeHeight;
            checkBox7.Checked = Settings.OverlaysVisible;
            checkBox8.Checked = Settings.DisplayGrid;

            button1.BackColor = Color.FromArgb(((byte)Settings.GridColor.X*255), 
                                               ((byte)Settings.GridColor.Y*255), 
                                               ((byte)Settings.GridColor.Z*255));

            ready = true;
        }

        public void UpdateVisibility()
        {
            map.heightmap.SetVisibilitySettings();
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.UnitsVisible = checkBox1.Checked;
            if (ready)
                UpdateVisibility();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.BuildingsVisible = checkBox2.Checked;
            if (ready)
                UpdateVisibility();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ObjectsVisible = checkBox3.Checked;
            if (ready)
                UpdateVisibility();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Settings.DecorationsVisible = checkBox4.Checked;
            if (ready)
                UpdateVisibility();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            Settings.LakesVisible = checkBox5.Checked;
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            Settings.VisualizeHeight = checkBox6.Checked;
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            Settings.OverlaysVisible = checkBox7.Checked;
            MainForm.mapedittool.update_render = true;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            Settings.DisplayGrid = checkBox8.Checked;
            MainForm.mapedittool.update_render = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(GridColorPicker.ShowDialog() == DialogResult.OK)
            {
                Settings.GridColor = new OpenTK.Vector4(
                    GridColorPicker.Color.R,
                    GridColorPicker.Color.G,
                    GridColorPicker.Color.B,
                    255) / 255f;
                button1.BackColor = GridColorPicker.Color;
                MainForm.mapedittool.update_render = true;
            }
        }
    }
}
