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
            [Serializable]
            public class GearData {
                public int Gear;
                public float Ratio;
            }

            public List<GearData> gears;

            public float ReverseGearRatio;
            [Tooltip("In Seconds")]
            public float shitTime;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Transmission;
            }

            public static TransmissionSO CreateDefault()
            {
                var trans = new TransmissionSO();
                trans.gears = new List<GearData>();
                trans.gears.Add(new GearData { Gear = 1, Ratio = 3.636f});
                trans.gears.Add(new GearData { Gear = 1, Ratio = 2.375f });
                trans.gears.Add(new GearData { Gear = 1, Ratio = 1.761f });
                trans.gears.Add(new GearData { Gear = 1, Ratio = 1.346f });
                trans.gears.Add(new GearData { Gear = 1, Ratio = 1.062f });
                trans.gears.Add(new GearData { Gear = 1, Ratio = 0.842f });
                trans.shitTime = 0.5f;

                trans.ReverseGearRatio = 3.545f;
                return trans;
            }
        }
    }
}