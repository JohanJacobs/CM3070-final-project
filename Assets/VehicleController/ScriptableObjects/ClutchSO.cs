using System;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Clutch", menuName = "Components/Clutch SO")]
        public class ClutchSO: ScriptableObjectBase
        {
            public FloatVariable engineTorque;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Clutch;
            }
            public static ClutchSO CreateDefault()
            {
                var clutch = new ClutchSO();
                return clutch;
            }
        }
    }
}