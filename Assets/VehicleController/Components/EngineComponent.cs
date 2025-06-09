using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        
        public class EngineComponent : IVehicleComponent,IDebugInformation
        {
            EngineSO config;
            FloatVariable throttle;
            FloatVariable engineIdleRPM;
            FloatVariable engineRedlineRPM;
            FloatVariable engineRPM;
            public float effectiveTorque => engineEffectiveTorque.Value;
            FloatVariable engineEffectiveTorque;

            float RPMtoRadians => Mathf.PI * 2f / 60f;
            float RadianstoRPM => 1f / RPMtoRadians;

            public EngineComponent(EngineSO config)
            {
                this.config = config;
                
                // setup variables
                this.throttle = config.throttleVariable;
                this.engineIdleRPM = config.engineIdleRPMVariable;
                this.engineRedlineRPM = config.engineRedlineRPMVariable;
                this.engineRPM = config.engineRPMVariable;                
                this.engineEffectiveTorque = config.engineEffectiveTorque;
            }

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Engine;
            public void Shutdown()
            {

            }

            public void Start()
            {
                engineRedlineRPM.Value = config.redlineRPM;
                engineIdleRPM.Value = config.idleRPM;

                engineRPM.Value = engineIdleRPM.Value;

                engineAngularVelocity = engineRPM.Value * RPMtoRadians;
            }

            public void Update(float dt)
            {
                EngineAcceleration(dt);
            }
            #endregion IVehicleComponent
            #region engine

            float startFriction = 50f;
            float frictionCoefficient = 0.02f;
            float engineInertia = 0.2f;
                                  
            float engineAngularVelocity = default;

            void EngineAcceleration(float dt)
            {
                var internalFriction = startFriction + engineRPM.Value * frictionCoefficient;
                var maximumEffectiveTorque = config.torqueCurve.Evaluate(engineRPM.Value);

                var currentTorque = (maximumEffectiveTorque + maximumEffectiveTorque) * throttle.Value;
                engineEffectiveTorque.Value = currentTorque - internalFriction;

                var engineAcceleration = (engineEffectiveTorque.Value / engineInertia) * dt;
                engineAngularVelocity = Mathf.Clamp(engineAngularVelocity + engineAcceleration, engineIdleRPM.Value * RPMtoRadians, engineRedlineRPM.Value * RPMtoRadians);
                engineRPM.Value = engineAngularVelocity * RadianstoRPM;        
            }

            public void DrawGizmos()
            {
                
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"throttle : {throttle.Value.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"engineAngularVelocity: {engineAngularVelocity.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"engineEffectiveTorque: {engineEffectiveTorque.Value.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"RPM: {engineRPM.Value.ToString("F3")}");
                return yOffset;
            }
            #endregion engine


        }

    }
}