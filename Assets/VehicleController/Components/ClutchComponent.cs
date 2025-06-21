using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {

        public class ClutchComponent : IVehicleComponent, IDebugInformation
        {
            ClutchSO config;               
            FloatVariable engineTorque;

            public ClutchComponent(ClutchSO config)
            {
                this.config = config;
                this.engineTorque = config.engineTorque;
            }

            #region Clutch Component
            float clutchCapacity = 1.3f;
            float clutchStiffness = 40f;
            float clutchDamping = 0.7f;

            float engineMaxTorque = 400f; // nm TODO: read from engine
            float engineMaxRPM = 7000f;   // rmp TODO: read from engine 
            float clutchMaxTorque => clutchCapacity * engineMaxTorque;

            float RadToRPM => 60f/(2 * Mathf.PI);
            float engineRPMRate=> MathHelper.MapAndClamp(engineAngularVelocity * RadToRPM,1000f,1300f,0f,1f);
            float NeutralGearCheck => transmissionRatio < float.Epsilon ? 1 : 0f;
            float clutchLock => Mathf.Min(engineRPMRate + NeutralGearCheck, 1f);
            float clutchSlip => (engineAngularVelocity - transmissionAngularVelocity) * MathHelper.Sign(Mathf.Abs(transmissionRatio));
                        
            float transmissionRatio = default;
            float engineAngularVelocity = default;
            public float clutchTorque { get; private set; }
            float transmissionAngularVelocity = default;
            public void Update(float transmissionVelocity, float transmissionRatio, float engineAngularVelocity)
            {
                this.transmissionAngularVelocity = transmissionVelocity;
                this.transmissionRatio = transmissionRatio;
                this.engineAngularVelocity = engineAngularVelocity;
                // clutch Damping

                var transferredClutchTorque = Mathf.Clamp(clutchSlip * clutchLock * clutchStiffness, -clutchMaxTorque, clutchMaxTorque);
                var damping = (clutchTorque - transferredClutchTorque) * clutchDamping;
                clutchTorque = transferredClutchTorque + damping;
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

            public void Update(float dt)
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
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  slip: {clutchSlip.ToString("F1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  lock: {clutchLock.ToString("F1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  stiffness: {clutchStiffness.ToString("F1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  torque: {clutchTorque.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  VeloTrans: {transmissionAngularVelocity.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  VeloEngine: {engineAngularVelocity.ToString("F3")}");

                return yOffset;
            }
            #endregion IDebugInformation
        }

    }
}