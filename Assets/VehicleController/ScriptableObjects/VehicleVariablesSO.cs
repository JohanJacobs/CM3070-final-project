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
            public FloatVariable engineCurrentRPM;
            public FloatVariable engineRedlineRPM;
            public FloatVariable engineIdleRPM;
            public FloatVariable engineStartFriction;
            public FloatVariable engineInternalFrictionCoefficient;
            public FloatVariable engineInertia;

            [Header("Transmission")]
            public StringVariable currentGearText;

        }
    }
}