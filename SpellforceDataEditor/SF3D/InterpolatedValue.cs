﻿/*
 * IInterpolatedValue<T> is an interface for an InterpolatedValue class
 * Classes implementing IInterpolatedValue must be able to add new entries at a given time, retrieve
 *      interpolated values at a given time, and return maximum time available
 * Currently this interface is implemented by InterpolatedVector3 and InterpolatedQuaternion
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace SpellforceDataEditor.SF3D
{
    public interface IInterpolatedValue<T>
    {
        void Add(T item, float t);
        T Get(float t);
        float GetMaxTime();
        int GetSizeBytes();
    }

    public class InterpolatedVector3: IInterpolatedValue<Vector3>
    {
        List<Vector3> value = new List<Vector3>();
        List<float> time = new List<float>();
        float max_time = -1;

        public void Add(Vector3 v, float t)
        {
            if (t >= max_time)
            {
                value.Add(v);
                time.Add(t);
                max_time = t;
            }
            else
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedVector3.Add(): Invalid time parameter (time = " + t.ToString() + ", max_time = " + max_time.ToString());
                throw new InvalidOperationException("Invalid time parameter (t <= max_time)");
            }
        }

        public Vector3 Get(float t)
        {
            int size = value.Count;
            if (size == 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedVector3.Get(): Data is empty!");
                throw new IndexOutOfRangeException("Array is empty");
            }
            if (size == 1)
                return value[0];

            if (t < 0)
                t = 0;
            if (t > max_time)
                t = max_time;

            for(int i = 0; i < time.Count; i++)
            {
                if(time[i] >= t)
                {
                    if (i == 0)
                        return value[0];

                    float t1 = time[i - 1];
                    return Vector3.Lerp(value[i - 1], value[i], (t - t1) / (time[i] - t1));
                }
            }

            LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedVector3.Get(): Data is malformed!!!!");
            throw new ArithmeticException("Invalid data");
        }

        public float GetMaxTime()
        {
            return max_time;
        }

        public int GetSizeBytes()
        {
            return 4 * time.Count + 12 * value.Count;
        }
    }

    public class InterpolatedQuaternion: IInterpolatedValue<Quaternion>
    {
        List<Quaternion> value = new List<Quaternion>();
        List<float> time = new List<float>();
        float max_time = -1;

        public void Add(Quaternion v, float t)
        {
            if (t >= max_time)
            {
                value.Add(v);
                time.Add(t);
                max_time = t;
            }
            else
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedQuaternion.Add(): Invalid time parameter (time = " + t.ToString() + ", max_time = " + max_time.ToString());
                throw new InvalidOperationException("Invalid time parameter (t <= max_time)");
            }
        }

        public Quaternion Get(float t)
        {
            int size = value.Count;
            if (size == 0)
            {
                LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedQuaternion.Get(): Data is empty!");
                throw new IndexOutOfRangeException("Array is empty");
            }
            if (size == 1)
                return value[0];

            if (t < 0)
                t = 0;
            if (t > max_time)
                t = max_time;

            for (int i = 0; i < time.Count; i++)
            {
                if (time[i] >= t)
                {
                    if (i == 0)
                        return value[0];

                    float t1 = time[i - 1];
                    return Quaternion.Slerp(value[i - 1], value[i], (t - t1) / (time[i] - t1));
                }
            }

            LogUtils.Log.Error(LogUtils.LogSource.SF3D, "InterpolatedQuaternion.Get(): Data is malformed!!!!");
            throw new ArithmeticException("Invalid data");
        }

        public float GetMaxTime()
        {
            return max_time;
        }

        public int GetSizeBytes()
        {
            return 4 * time.Count + 16 * value.Count;
        }
    }
}
