using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
    namespace VehicleComponent
    {
        public interface IElectronicStabilityControl
        {
            public float CalculateWheelBrakeForce(WheelID wheel);
        }

        public class ElectronicStabilityControlComponent : IVehicleComponent<ElectronicStabilityControlComponentParams>,IDebugInformation,IElectronicStabilityControl
        {

            BoolVariable enabled;            
            FloatVariable activateAngleDEG;
            FloatVariable activateSpeedKMH;
            FloatVariable maxBrakeAngleDEG;

            ElectronicStabilityControlSO config;
            #region IElectronicStabilityControl

            float FrontLeftBrakeForce;
            float FrontRightBrakeFroce;
            public float CalculateWheelBrakeForce(WheelID wheel)
            {
                // TODO: Make dictionary if i do 4 wheels
                switch (wheel) 
                {
                    case WheelID.LeftFront: return FrontLeftBrakeForce;
                    case WheelID.RightFront: return FrontRightBrakeFroce;                    
                }

                return 0f;
            }
            #endregion IElectronicStabilityControl

            #region ElectrnoicStabilityControlComponent
            public ElectronicStabilityControlComponent(ElectronicStabilityControlSO config, VehicleVariablesSO variables)
            {
                this.config = config;

                //variables
                this.enabled = variables.ESCEnabled;
                this.activateAngleDEG = variables.ESCActivateAngle;
                this.activateSpeedKMH = variables.ESCActivateSpeed;
                this.maxBrakeAngleDEG = variables.ESCMaxBrakeAngle;
                
            }

            private void CalculateFrontBrakeForceFromSliding(IBodyComponent body, IBrake leftBrake, IBrake rightBrake)
            {
                var absDeg = Mathf.Abs(body.DriftAngleDEG);

                // linearly increase the strength
                float ESCStrength = MathHelper.MapAndClamp(absDeg, activateAngleDEG.Value, maxBrakeAngleDEG.Value, 0f, 1f);

                if (absDeg > activateAngleDEG.Value && body.SpeedKMH > activateSpeedKMH.Value && enabled)
                {
                    if (body.DriftAngleDEG > 0f)
                    {
                        // slip to the left
                        FrontRightBrakeFroce = rightBrake.MaxTorque * ESCStrength;
                        return;
                    }
                    if (body.DriftAngleDEG < 0f)
                    {
                        // slip to the right 
                        FrontLeftBrakeForce = leftBrake.MaxTorque * ESCStrength;                        
                        return;
                    }
                }

                // reset forces 
                FrontLeftBrakeForce = 0f;
                FrontRightBrakeFroce = 0f;
            }
            #endregion ElectrnoicStabilityControlComponent

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.ESC;
            public void Start()
            {
                this.activateAngleDEG.Value = this.config.ActivateBrakeAngle;                
                this.maxBrakeAngleDEG.Value = this.config.MaxBrakeAngle;
                this.activateSpeedKMH.Value = this.config.ActivateSpeed;
                this.enabled.Value = this.config.Enabled;
            }

            public void Shutdown()
            {

            }

            public void Step(ElectronicStabilityControlComponentParams parameters)
            {
                CalculateFrontBrakeForceFromSliding(parameters.body,parameters.leftBrake,parameters.rightBrake);                   
            }
            #endregion IVehicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {
                
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {

                return yOffset;
            }
            #endregion IDebugInformation

        }

        public class ElectronicStabilityControlComponentParams
        {
            public ElectronicStabilityControlComponentParams(IBodyComponent body,IBrake leftBrake, IBrake rightBrake)
            {
                this.body = body;
                this.leftBrake = leftBrake;
                this.rightBrake = rightBrake;
            }
            public IBodyComponent body;
            public IBrake leftBrake;
            public IBrake rightBrake;
        }

    }
}