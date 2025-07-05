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
            public FloatVariable transmissionEfficiency;
            public StringVariable currentGearText;
            public FloatVariable gearCount;
            public FloatVariable gearShiftTime;
            public FloatVariable gearRatioNeutral;
            public FloatVariable gearRatioFirst;
            public FloatVariable gearRatioSecond;
            public FloatVariable gearRatioThird;
            public FloatVariable gearRatioForth;
            public FloatVariable gearRatioFifth;
            public FloatVariable gearRatioSixth;
            public FloatVariable gearRatioReverse;

            [Header("Differential")]
            public FloatVariable differentialGear;

            [Header("Brakes")]
            public FloatVariable brakeTorque;
            public FloatVariable brakeBalance;

            [Header("Aero")]
            public FloatVariable DragCoefficient;
            public FloatVariable LiftCoefficient;
            public FloatVariable SurfaceAera;

            [Header("Anti Rollbar")]
            public FloatVariable AntiRollbarForceFront;
            public FloatVariable AntiRollbarForceRear;

            [Header("Suspension - Front")]
            public FloatVariable RestLengthFront;
            public FloatVariable SpringForceFront;
            public FloatVariable DamperForceFront;

            [Header("Suspension - Rear")]
            public FloatVariable RestLengthRear;
            public FloatVariable SpringForceRear;
            public FloatVariable DamperForceRear;


            [Header("Body")]
            public FloatVariable BodyMass;
            public FloatVariable BodyDrag;

            [Header("Tires")]
            public ScriptableObjectVariable Tires;
        }
    }
}