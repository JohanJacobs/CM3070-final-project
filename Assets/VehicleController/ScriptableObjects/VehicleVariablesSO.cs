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

            [Header("Clutch")]
            public FloatVariable clutchCapacity;
            public FloatVariable clutchDampingRate;
            public FloatVariable clutchStiffness;

            [Header("Transmission")]
            public StringVariable currentGearText;

            [Header("Differential")]
            public FloatVariable differentialGear;

            [Header("Brakes")]
            public FloatVariable brakeTorque;
            public FloatVariable brakeBalance;

            [Header("Aero")]
            public FloatVariable DragCoefficient;
            public FloatVariable LiftCoefficient;
            public FloatVariable SurfaceAera;
        }
    }
}