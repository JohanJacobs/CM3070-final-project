using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Aero", menuName = "Components/Aero SO")]
        public class AeroSO: ScriptableObjectBase
        {
            // wing efficiency (We) = CL/CD
            // => 1/CD = We/CL
            // => CD = CL/We
            // https://occamsracers.com/2023/08/08/car-wings-examined/
            // 

            [Tooltip("Meter squared")]
            public float surfaceArea; // meter squared (Width * high) 
            
            [UnityEngine.Range(0.0f, 10.0f)]
            public float LiftCoefficient;

            [UnityEngine.Range( 0.0f,10.0f)]
            public float dragCoefficient; 

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Aero;
            }

            public static AeroSO CreateDefault()
            {
                var aero = new AeroSO();
                aero.surfaceArea = 0f;
                aero.LiftCoefficient = 0f;
                return aero;
            }
        }
    }
}