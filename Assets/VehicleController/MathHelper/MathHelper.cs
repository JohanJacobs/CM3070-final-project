using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

namespace vc
{
    public class MathHelper
    {
        public static float Sign(float value)
        {
            if (Mathf.Abs(value) < float.Epsilon)
                return 0f;

            if (value < 0f)
                return -1f;

            return 1f;
        }

        public static float MapAndClamp(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            var v = MathHelper.Remap(value, fromMin, fromMax, toMin, toMax);

            if (v < toMin)
                return toMin;

            if (v > toMax)
                return toMax;

            return v;
        }
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        }

        public static float SafeDivide(float a, float b)
        {
            if (b < float.Epsilon)
                return 0f;
            return a / b;
        }


    }
}