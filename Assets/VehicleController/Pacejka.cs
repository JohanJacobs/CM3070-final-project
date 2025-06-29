using System;
using UnityEngine;

namespace vc
{

    public static class Pacjeka
    {
        //https://www.edy.es/dev/docs/pacejka-94-parameters-explained-a-comprehensive-guide/
        static float A_Adherent_mu = 0.95f;
        static float B_Stiffness = 10f;
        static float C_Shape_Long = 1.9f;
        static float C_Shape_Lat = 1.375f;
        static float D_Peak = 0.783f;
        static float E_Curvature = 09.88f;

        public static float quickPacjekaLong(float slipratio)
        {
            return quickPacejka(slipratio);
        }
        public static float quickPacjekaLat(float slipAngle)
        {
            return quickPacejka(slipAngle, false);
        }

        // Pacejka formula
        public static float quickPacejka(float slipratio, bool useLongitudinal = true)
        {
            float b = B_Stiffness;
            float c = (useLongitudinal ? C_Shape_Long : C_Shape_Lat);
            float d = D_Peak;
            float e = E_Curvature;

            float result = d * Mathf.Sin(c * Mathf.Atan(b * slipratio - e * (b * slipratio - Mathf.Atan(b * slipratio))));
            return result;

        }

    }
}