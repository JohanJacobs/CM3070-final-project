using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Vehicle", menuName = "Components/VehicleSO")]
        public class VehicleSO : ScriptableObjectBase
        {
            public static VehicleSO CreateDefault()
            {
                return new VehicleSO();
            }
        }
    }
}