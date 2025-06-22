using UnityEngine;

namespace vc
{
    public class PhysicsHelper
    {
        public class Conversions
        {
            static readonly float ms_kmh_conversion = 3.6f;
            public static float MStoKMH(float metersPerSecond) => metersPerSecond * ms_kmh_conversion; // km/h

            static float radians_to_revolutions_per_minute = 60f / (2 * Mathf.PI);
            public static float RadToRPM(float radians) => radians * radians_to_revolutions_per_minute;
            public static float RPMToRad(float rpm) => rpm / radians_to_revolutions_per_minute;

        }

        /* CalculateDragForce 
            air density : kg/m³
            area : m²
            velocity : ms
            coefficient of drag : constant
            https://www.grc.nasa.gov/www/k-12/VirtualAero/BottleRocket/airplane/drageq.html
            // typical values is 0.2 to 0.4 //https://en.wikipedia.org/wiki/Automobile_drag_coefficient#:~:text=The%20average%20modern%20automobile%20achieves,=0.285%20to%20Cd=0.315.
        */
        public static float CalculateDrag(float velocityMS, float areaM, float dragCoefficient, float airDensity = 1.225f)
        {
            return 0.5f * airDensity * (velocityMS * velocityMS) * areaM * dragCoefficient;
        }


    }
}
