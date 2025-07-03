using System;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Clutch", menuName = "Components/Clutch SO")]
        public class ClutchSO: ScriptableObjectBase
        {
            public float clutchCapacity;    // NM torque that can be transfered by the clutch
            public float clutchStiffness;   // 
            public float clutchDampingRate; // 
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