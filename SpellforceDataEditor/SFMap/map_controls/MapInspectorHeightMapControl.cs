﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpellforceDataEditor.SFMap.map_controls
{
    public partial class MapInspectorHeightMapControl : MapInspectorBaseControl
    {
        public MapInspectorHeightMapControl()
        {
            InitializeComponent();
        }

        public override void OnMouseDown(SFCoord clicked_pos, MouseButtons button)
        {
            float strength = Utility.TryParseFloat(StrengthTextBox.Text, 0f);
            int size = (int)Math.Ceiling(BrushControl.brush.size);
            BrushControl.brush.center = clicked_pos;
            SFCoord topleft = new SFCoord(clicked_pos.x - size, clicked_pos.y - size);
            SFCoord bottomright = new SFCoord(clicked_pos.x + size, clicked_pos.y + size);
            if (topleft.x < 0)
                topleft.x = 0;
            if (topleft.y < 0)
                topleft.y = 0;
            if (bottomright.x >= map.width)
                bottomright.x = (short)(map.width - 1);
            if (bottomright.y >= map.height)
                bottomright.y = (short)(map.height - 1);

            if (button == MouseButtons.Left)
            {
                switch (ComboDrawMode.SelectedIndex)
                {
                    case 0:  // add
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {

                                float cell_strength = BrushControl.brush.GetStrengthAt(new SFCoord(i, j));
                                int fixed_j = map.height - j - 1;
                                if (map.heightmap.lake_data[fixed_j * map.width + i] != 0)
                                    continue;
                                map.heightmap.height_data[fixed_j * map.width + i] += (ushort)(strength * cell_strength);
                                if (map.heightmap.height_data[fixed_j * map.width + i] > 30000)
                                    map.heightmap.height_data[fixed_j * map.width + i] = 30000;
                            }
                        }
                        break;
                    case 1:  // sub
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = BrushControl.brush.GetStrengthAt(new SFCoord(i, j));
                                int fixed_j = map.height - j - 1;
                                if (map.heightmap.lake_data[fixed_j * map.width + i] != 0)
                                    continue;
                                if (strength * cell_strength >= map.heightmap.height_data[fixed_j * map.width + i])
                                    map.heightmap.height_data[fixed_j * map.width + i] = 0;
                                else
                                    map.heightmap.height_data[fixed_j * map.width + i] -= (ushort)(strength * cell_strength);
                            }
                        }
                        break;
                    case 2:  // set
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                int fixed_j = map.height - j - 1;
                                if (map.heightmap.lake_data[fixed_j * map.width + i] != 0)
                                    continue;
                                map.heightmap.height_data[fixed_j * map.width + i] = (ushort)(strength);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else if(button == MouseButtons.Right)
            {
                switch (ComboDrawMode.SelectedIndex)
                {
                    case 0:  // add
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {

                                float cell_strength = BrushControl.brush.GetStrengthAt(new SFCoord(i, j));
                                int fixed_j = map.height - j - 1;
                                if (map.heightmap.lake_data[fixed_j * map.width + i] != 0)
                                    continue;
                                if (strength * cell_strength >= map.heightmap.height_data[fixed_j * map.width + i])
                                    map.heightmap.height_data[fixed_j * map.width + i] = 0;
                                else
                                    map.heightmap.height_data[fixed_j * map.width + i] -= (ushort)(strength * cell_strength);
                            }
                        }
                        break;
                    case 1:  // sub
                        for (int i = topleft.x; i <= bottomright.x; i++)
                        {
                            for (int j = topleft.y; j <= bottomright.y; j++)
                            {
                                float cell_strength = BrushControl.brush.GetStrengthAt(new SFCoord(i, j));
                                int fixed_j = map.height - j - 1;
                                if (map.heightmap.lake_data[fixed_j * map.width + i] != 0)
                                    continue;
                                map.heightmap.height_data[fixed_j * map.width + i] += (ushort)(strength * cell_strength);
                                if (map.heightmap.height_data[fixed_j * map.width + i] > 30000)
                                    map.heightmap.height_data[fixed_j * map.width + i] = 30000;
                            }
                        }
                        break;
                    case 2:  // set
                        SFCoord inv_clicked_pos = new SFCoord(clicked_pos.x, map.height - clicked_pos.y - 1);
                        StrengthTextBox.Text = map.heightmap.height_data[inv_clicked_pos.y * map.width + inv_clicked_pos.x].ToString();
                        return;
                    default:
                        break;
                }
            }

            map.heightmap.RebuildGeometry(topleft, bottomright);
        }

        private void ComboDrawMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboDrawMode.SelectedIndex == -1)
                return;
            if (ComboDrawMode.SelectedIndex == 2)
                LabelStrength.Text = "Value";
            else
                LabelStrength.Text = "Strength";
        }
    }
}
