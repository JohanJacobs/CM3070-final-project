using System;
using UnityEngine;

namespace vc
{

    public static class Pacjeka
    {
        //https://www.edy.es/dev/docs/pacejka-94-parameters-explained-a-comprehensive-guide/        
        [Serializable]
        public class PacjekaConfig
        {
            [HideInInspector]
            public float A_MU;
            public float B_Stiffness;
            public float C_Shape;
            public float D_Peak;
            public float E_Curvature;

            public PacjekaConfig(float mu = 1f, float stiffness = 10f, float shape = 1.375f, float peak = 0.783f,float curvature = 0.98f)
            {
                this.A_MU = mu;
                this.B_Stiffness = stiffness;
                this.C_Shape = shape;
                this.D_Peak = peak;
                this.E_Curvature = curvature;
            }
        }

        // Pacejka formula
        public static float MagicFormula(float slipratio, PacjekaConfig config)
        {
            float b = config.B_Stiffness;
            float c = config.C_Shape;
            float d = config.D_Peak;
            float e = config.E_Curvature;

            float result = d * Mathf.Sin(c * Mathf.Atan(b * slipratio - e * (b * slipratio - Mathf.Atan(b * slipratio))));
            return result * config.A_MU;
        }
    }
}