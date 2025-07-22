using UnityEngine;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
    namespace VehicleComponent
    {
        public interface IElectronicStabilityControl
        {
            public float FrontLeftBrakeForce { get; }
            public float FrontRightBrakeFroce { get; }
        }

        public class ElectronicStabilityControlComponent : IVehicleComponent<ElectronicStabilityControlComponentParams>,IDebugInformation,IElectronicStabilityControl
        {

            float escActivateAngleDEG = 10f;
            float escActiveSpeedKMH = 20f;
            float escMaxAtAngle = 80f;

            #region IElectronicStabilityControl
            public float FrontLeftBrakeForce { get; private set; }
            public float FrontRightBrakeFroce { get; private set; }
            #endregion IElectronicStabilityControl

            #region ElectrnoicStabilityControlComponent
            public ElectronicStabilityControlComponent(VehicleVariablesSO variables)
            {

            }

            private void CalculateFrontBrakeForceFromSliding(IBodyComponent body, IBrake leftBrake, IBrake rightBrake)
            {
                var absDeg = Mathf.Abs(body.DriftAngleDEG);
                // calculate the angle the body is moving 


                // linearly increase the strength
                float ESCStrength = MathHelper.MapAndClamp(absDeg, escActivateAngleDEG, escMaxAtAngle, 0f, 1f);

                if (absDeg > escActivateAngleDEG && body.SpeedKMH > escActiveSpeedKMH)
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