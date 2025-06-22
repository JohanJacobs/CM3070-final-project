using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {

        public class ClutchComponent : IVehicleComponent<ClutchComponenetStepParamters>, IDebugInformation
        {
            ClutchSO config;                           
            #region Clutch Component
            public ClutchComponent(ClutchSO config)
            {
                this.config = config;                
            }

            float clutchCapacity = 1.3f;
            float clutchStiffness = 40f;
            float clutchDampingRate = 0.7f;

            float engineMaxTorque = 400f; // nm TODO: read from engine            
            float clutchMaxTorque => clutchCapacity * engineMaxTorque;
                        
            float engineRPMRate=> MathHelper.MapAndClamp(PhysicsHelper.Conversions.RadToRPM(engineAngularVelocity), 1000f, 1300f, 0f, 1f); // Region in which the clutch slips
            float transmissionInNeutralGear => Mathf.Abs(transmissionRatio) < float.Epsilon ? 1 : 0f;
            float transferredClutchTorque => Mathf.Clamp(clutchSlip * clutchLock * clutchStiffness, -clutchMaxTorque, clutchMaxTorque);
            float clutchLock = default;
            float clutchSlip = default;
                        
            float transmissionRatio = default;
            float engineAngularVelocity = default;
            public float clutchTorque { get; private set; }
            float transmissionAngularVelocity = default;
            public void Update(float transmissionVelocity, float transmissionRatio, float engineAngularVelocity)
            {
                this.transmissionAngularVelocity = transmissionVelocity;
                this.transmissionRatio = transmissionRatio;
                this.engineAngularVelocity = engineAngularVelocity;

                clutchSlip = (engineAngularVelocity - transmissionAngularVelocity) * MathHelper.Sign(Mathf.Abs(transmissionRatio));
                clutchLock = Mathf.Min((engineRPMRate + transmissionInNeutralGear), 1f);
                                
                var clutchDamping = (clutchTorque - transferredClutchTorque) * clutchDampingRate;
                clutchTorque = transferredClutchTorque + clutchDamping;
            }
                        
            #endregion Clutch Component

            #region IVerhicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Differential;
            public void Shutdown()
            {

            }

            public void Start()
            {

            }

            public void Step(ClutchComponenetStepParamters parameters)
            {

            }
            #endregion IVerhicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"CLUTCH");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Slip: {clutchSlip.ToString("F1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Lock: {clutchLock.ToString("F1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Stiffness: {clutchStiffness.ToString("F1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Torque: {clutchTorque.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Trans.Velo: {transmissionAngularVelocity.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Engine.Velo: {engineAngularVelocity.ToString("F3")}");

                return yOffset;
            }
            #endregion IDebugInformation


        }
        #region Clutch Componenet Paramters
        public class ClutchComponenetStepParamters
        {

        }
        #endregion Clutch Componenet Paramters

    }
}