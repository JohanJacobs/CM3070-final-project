using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Aero", menuName = "Components/Aero SO")]
        public class AeroSO: ScriptableObjectBase
        {
            [Tooltip("Meter squared")]
            public float surfaceArea; // meter squared
            public float angle; // degrees

            [UnityEngine.Range(0.5f, 2.0f)]
            public float LiftCoefficient;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Aero;
            }

            public static AeroSO CreateDefault()
            {
                var aero = new AeroSO();
                aero.surfaceArea = 0f;
                aero.angle = 0f;
                aero.LiftCoefficient = 0f;
                return aero;
            }
        }
    }
}