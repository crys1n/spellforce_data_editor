﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SF3D
{
    public struct CompressedMatrix
    {
        Vector3 position;
        Quaternion rotation;

        public CompressedMatrix(Vector3 v, Quaternion q)
        {
            position = v;
            rotation = q;
        }

        public Matrix4 to_mat4()
        {
            Matrix3 mat_rot = Matrix3.CreateFromQuaternion(rotation);
            Matrix4 mat4 = new Matrix4();
            mat4.Row0 = new Vector4(mat_rot.Row0, 0);
            mat4.Row1 = new Vector4(mat_rot.Row1, 0);
            mat4.Row2 = new Vector4(mat_rot.Row2, 0);
            mat4.Row3 = new Vector4(position, 1);
            return mat4;

        }
    }
}
