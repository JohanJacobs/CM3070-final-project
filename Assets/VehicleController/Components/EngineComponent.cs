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
            AnimationCurve torqueCurve;
            float idleRPM;
            float redlineRPM;
            float RPMtoRadians => Mathf.PI * 2f / 60f;
            float RadianstoRPM => 1f / RPMtoRadians;

            public EngineComponent(EngineSO config)
            {
                this.config = config;
                
                // setup variables
                this.throttle = config.throttleVariable;
                this.torqueCurve = config.torqueCurve;
                this.idleRPM = config.idleRPM;
                this.redlineRPM = config.redlineRPM;
            }

            #region engine
            public void Update(float dt, float loadTorque)
            {
                UpdateEngineAcceleration(dt, loadTorque);
            }

            float startFriction = 50f;          // nm
            float frictionCoefficient = 0.02f;
            float engineInertia = 0.2f;
            float engineAngularVelocity = default;
            float engineRPM = default;
            float internalFriction => startFriction + (engineRPM * frictionCoefficient); // nm
            float rpmTorque => torqueCurve.Evaluate(engineRPM); // nm
            float effectiveTorqueProduced => (rpmTorque + internalFriction) * throttle.Value; // nm
            float engineEffectiveTorque => effectiveTorqueProduced - internalFriction; // nm            
            void UpdateEngineAcceleration(float dt,float loadTorque)
            {                
                var engineAcceleration = ((engineEffectiveTorque - loadTorque) / engineInertia) * dt; // Rad/s²

                engineAngularVelocity = Mathf.Clamp(engineAngularVelocity + engineAcceleration, idleRPM*RPMtoRadians, redlineRPM*RPMtoRadians); // rad/s

                engineRPM = engineAngularVelocity * RadianstoRPM; // can read this from the wheels and the current gears ?
            }
            #endregion engine

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

            #region IDebugInformation
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
            #endregion IDebugInformation


        }

    }
}