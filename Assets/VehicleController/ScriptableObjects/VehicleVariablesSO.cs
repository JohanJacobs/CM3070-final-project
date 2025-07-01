using UnityEngine;

namespace vc
{
    namespace VehicleConfiguration
    {
        [CreateAssetMenu(fileName = "VehicleVariables", menuName = "Vehicle Variable SO")]
        public class VehicleVariablesSO: ScriptableObjectBase
        {
            [Header("Input")]
            public FloatVariable steer;
            public FloatVariable throttle;
            public FloatVariable brake;
            public FloatVariable handBrake;
            public FloatVariable gearUp;
            public FloatVariable gearDown;

            [Header("General")]
            public FloatVariable speedKMH;

            [Header("Engine")]
            public FloatVariable currentRPM;
            public FloatVariable redlineRPM;
            public FloatVariable idleRPM;

            [Header("Transmission")]
            public StringVariable currentGearText;



        }
    }
}