using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

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

    public static float MapAndClamp(float value, float fromMin, float fromMax, float toMin,float toMax)
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

    // CalculateDragForce 
    // air density : kg/m³
    // area : m²
    // velocity : ms
    // coefficient of drag : constant
    // https://www.grc.nasa.gov/www/k-12/VirtualAero/BottleRocket/airplane/drageq.html
    //// typical values is 0.2 to 0.4 //https://en.wikipedia.org/wiki/Automobile_drag_coefficient#:~:text=The%20average%20modern%20automobile%20achieves,=0.285%20to%20Cd=0.315.
    public static float CalculateDrag(float velocityMS, float areaM, float dragCoefficient, float airDensity=1.225f)
    {
        return 0.5f * airDensity * (velocityMS * velocityMS) * areaM * dragCoefficient;
    }
}
