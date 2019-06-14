﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SFMap
{
    public class SFMapSelectionHelper
    {
        enum SelectionType { NONE, UNIT, BUILDING, OBJECT, INTERACTIVE_OBJECT }

        static SF3D.SFModel3D selection_mesh = new SF3D.SFModel3D();
        SFMap map = null;
        SF3D.ObjectSimple3D sel_obj = null;

        SFMapUnit selected_unit = null;
        SFMapBuilding selected_building = null;
        SFMapObject selected_object = null;
        SFMapInteractiveObject selected_interactive_object = null;
        SelectionType selection_type = SelectionType.NONE;

        public SFMapSelectionHelper()
        {
            // generate selection 3d model
            Vector3[] vertices = new Vector3[8];
            Vector2[] uvs = new Vector2[8];
            Vector4[] colors = new Vector4[8];
            Vector3[] normals = new Vector3[8];

            vertices[0] = new Vector3(-0.5f, 0.1f, -0.5f);
            vertices[1] = new Vector3(-0.5f, 0.1f, 0.5f);
            vertices[2] = new Vector3(0.5f, 0.1f, -0.5f);
            vertices[3] = new Vector3(0.5f, 0.1f, 0.5f);
            vertices[4] = new Vector3(-0.4f, 0.1f, -0.4f);
            vertices[5] = new Vector3(-0.4f, 0.1f, 0.4f);
            vertices[6] = new Vector3(0.4f, 0.1f, -0.4f);
            vertices[7] = new Vector3(0.4f, 0.1f, 0.4f);
            for (int i = 0; i < 8; i++)
            {
                colors[i] = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
                normals[i] = new Vector3(0.0f, 1.0f, 0.0f);
            }

            uint[] indices = { 4, 0, 1,   1, 5, 4,   5, 1, 3,   3, 7, 5,
                               7, 3, 2,   2, 6, 7,   6, 2, 0,   0, 4, 2 };

            selection_mesh.CreateRaw(vertices, uvs, colors, normals, indices, "");
            SFResources.SFResourceManager.Models.AddManually(selection_mesh, "_SELECTION_");
        }

        public void AssignToMap(SFMap _map)
        {
            map = _map;
            map.render_engine.scene_manager.AddObjectStatic("_SELECTION_", "", "_SELECTION_");
            sel_obj = map.render_engine.scene_manager.objects_static["_SELECTION_"];
            sel_obj.Rotation = Quaternion.FromEulerAngles(0, (float)Math.PI / 2, 0);
        }

        public void SetSelectionPosition(SFCoord pos)
        {
            float z = map.heightmap.GetZ(pos) / 100.0f;
            sel_obj.Position = new OpenTK.Vector3((float)pos.x, (float)z, (float)(map.height - pos.y - 1));
        }

        public void SetSelectionVisibility(bool vis)
        {
            sel_obj.Visible = vis;
        }

        public void SetSelectionScale(float s)
        {
            sel_obj.Scale = new Vector3(s, s, s);
        }

        public void CancelSelection()
        {
            selected_unit = null;
            selected_building = null;
            selected_object = null;
            selected_interactive_object = null;
            selection_type = SelectionType.NONE;
        }

        public void SelectUnit(SFMapUnit unit)
        {
            CancelSelection();
            selection_type = SelectionType.UNIT;
            selected_unit = unit;
            SetSelectionScale(1.0f);
        }

        // should be run once per render tick
        public void UpdateSelection()
        {
            SetSelectionVisibility(selection_type != SelectionType.NONE);
            if(selection_type == SelectionType.UNIT)
            {
                if (selected_unit != null)
                    SetSelectionPosition(selected_unit.grid_position);
            }
            // todo: add more selection types
        }

        public void Dispose()
        {
            map.render_engine.scene_manager.DeleteObject("_SELECTION_");
        }
    }
}
