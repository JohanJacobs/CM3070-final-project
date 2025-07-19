using System;
using System.Collections.Generic;
using UnityEngine;
using vc.VehicleComponent;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Transmission", menuName = "Components/Transmission SO")]
        public class TransmissionSO : ScriptableObjectBase
        {
            [Tooltip("Type")]
            public TransmissionComponent.TransmissionType TransmissionType;

            [Tooltip("Seconds")]
            public float gearShiftTime;

            [Tooltip("% of Torque conversion efficiency"),Range(0,1)]
            public float efficiency;

            public float reverseGearRatio;
            public float[] gearRatios;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Transmission;
            }

            public static TransmissionSO CreateDefault()
            {
                var trans = new TransmissionSO();
                return trans;
            }

        }
    }
}