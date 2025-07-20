using UnityEngine;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;
using static vc.VehicleComponent.DiffernetialTypes;

namespace vc
{
    namespace VehicleComponent
    {
        public class DifferentialComponent:IVehicleComponent<DifferentialComponenetStepParameters>,IDebugInformation,IRatio
        {
            public enum DifferentialType
            {
                Standard,
                TorqueSensing
            }

            public float Ratio => ratio.Value;

            DifferentialSO config;
            DifferentialType diffType = DifferentialType.Standard;
            int connectedWheels;
            FloatVariable ratio;
            float[] wheelOutput;
            public DifferentialComponent(DifferentialSO config, int connectedWheels, VehicleVariablesSO variables)
            {
                this.config = config;
                this.ratio = variables.differentialGear;
                
                this.connectedWheels = connectedWheels;
                this.wheelOutput = new float[connectedWheels];
            }

            #region Differential Component            

            // torque sensing differential sending more torque to the wheel that is slower as it has more grip.
            public float[] CalculateWheelOutputTorque(float transmissionTorque, WheelComponent leftWheel, WheelComponent rightWheel, float dt) 
            {
                switch(diffType)
                {
                    case DifferentialType.Standard: 
                        return DiffernetialTypes.StandardDifferential(connectedWheels, ratio.Value, transmissionTorque, leftWheel, rightWheel, dt);
                        
                    case DifferentialType.TorqueSensing: 
                        return DiffernetialTypes.TorqueSensingDiffernetial(connectedWheels, ratio.Value, transmissionTorque, leftWheel, rightWheel, dt);

                }

                // default case for invalid differential types.
                return DiffernetialTypes.StandardDifferential(connectedWheels, ratio.Value, transmissionTorque, leftWheel, rightWheel, dt);
            }

            float transmissionVelo = default;
            public float CalculateTransmissionVelocity(float leftWheel, float rightWheel)
            {
                float avgVelo = (leftWheel + rightWheel ) / 2f;
                transmissionVelo = avgVelo * ratio.Value;
                return transmissionVelo;
            }
            #endregion Differential Component


            #region IVerhicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Differential;
            public void Shutdown()
            {
                
            }

            public void Start()
            {
                this.ratio.Value =  this.config.GearRatio;
            }

            public void Step(DifferentialComponenetStepParameters parameters)
            {
                
            }
            #endregion IVerhicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {
                
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"DIFFERENTIAL");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Ratio : {ratio}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Wheel Torque: {wheelOutput[0].ToString("F2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Tansmission Velocity: {transmissionVelo.ToString("F2")}");

                return yOffset;
            }
            #endregion IDebugInformation
        }
        

        class DiffernetialTypes
        {

            #region TorqueSensingStandardDifferential
            static float CalcualteAngularVelocityDelta(WheelComponent left, WheelComponent right) => (left.wheelAngularVelocity - right.wheelAngularVelocity) * 0.5f;
            static float AverageWheelInertia(WheelComponent left, WheelComponent right) => (left.GetInertia + right.GetInertia) * 0.5f;
            public static float[] TorqueSensingDiffernetial(float connectedWheels, float ratio, float transmissionTorque, WheelComponent leftWheel, WheelComponent rightWheel, float dt)
            {
                float velocityDelta = CalcualteAngularVelocityDelta(leftWheel, rightWheel) / dt;
                float lockTorque = velocityDelta * AverageWheelInertia(leftWheel, rightWheel); // torque needed to align the wheels in rotation and redistribute torque

                float wheelTorque = (transmissionTorque * ratio) / connectedWheels;

                return new float[2] { wheelTorque - lockTorque, wheelTorque + lockTorque };
            }
            #endregion TorqueSensingStandardDifferential

            #region StandardDifferential
            // Open Differential
            public static float[] StandardDifferential(float connectedWheels, float ratio, float transmissionTorque, WheelComponent leftWheel, WheelComponent rightWheel, float dt)
            {
                float wheelTorque = (transmissionTorque * ratio) / connectedWheels;
                var wheelOutput = new float[2] {wheelTorque, wheelTorque};
                return wheelOutput;
            }
            #endregion StandardDifferential


        }
        
        #region Differential Componenet Step Parameters
        public class DifferentialComponenetStepParameters
        { }
        #endregion Differential Componenet Step Parameters
    }
}