using UnityEngine;
using vc.VehicleComponent;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "AntiRollbar", menuName = "Components/AntiRollbar SO")]
        public class AntiRollbarSO: ScriptableObjectBase
        {
            public RollbarComponent.RollbarPosition position;
            public float rollbarStrength; // NM

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.AnitRollbar;
            }

            public static AntiRollbarSO CreateDefault()
            {
                var arb = new AntiRollbarSO();
                return arb;
            }
        }
    }
}