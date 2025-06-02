using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Differential", menuName = "Components/Differential SO")]
        public class DifferentialSO: ScriptableObjectBase
        {
            public float GearRatio;


            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Differential;
            }

            public static DifferentialSO CreateDefault()
            {
                var diff = new DifferentialSO();

                diff.GearRatio = 3.9f;
                return diff;
            }
        }
    }
}