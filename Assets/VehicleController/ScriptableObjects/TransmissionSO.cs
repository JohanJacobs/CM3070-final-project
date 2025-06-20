using System;
using System.Collections.Generic;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Transmission", menuName = "Components/Transmission SO")]
        public class TransmissionSO: ScriptableObjectBase
        {
            public float GearRatio;
            public FloatVariable gearUpInputVariable;
            public FloatVariable gearDownInputVariable;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Transmission;
            }

            public static TransmissionSO CreateDefault()
            {
                var trans = new TransmissionSO();
                trans.GearRatio= 1f;
                return trans;
            }
        }
    }
}