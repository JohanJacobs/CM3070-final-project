using System;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Clutch", menuName = "Components/Clutch SO")]
        public class ClutchSO: ScriptableObjectBase
        {
            [Range(0f, 0.5f)]
            public float Intertia;// kg/m2
            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Clutch;
            }
            public static ClutchSO CreateDefault()
            {
                var clutch = new ClutchSO();
                clutch.Intertia = 0.15f;
                return clutch;
            }
        }
    }
}