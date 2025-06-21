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

            }

            #region engine
            public void Update(float dt, float loadTorque)
            {
                UpdateEngineAcceleration(dt, loadTorque);
            }

            float startFriction = 50f;          // nm
            float frictionCoefficient = 0.02f;
            float engineInertia = 0.2f;

            float maxEffectiveTorque;
            float engineRPM;
            float engineInternalFriction;
            float currentInitialTorque;
            float currentEffectiveTorque;
            float engineEffectiveTorque; // saved in UE5
            float acceleration;
            float angularVelocityDelta;
            public float engineAngularVelocity;
            void UpdateEngineAcceleration(float dt,float loadTorque)
            {

                maxEffectiveTorque = torqueCurve.Evaluate(engineRPM);
                engineInternalFriction = startFriction + (engineRPM * frictionCoefficient);
                currentInitialTorque = (maxEffectiveTorque + engineInternalFriction) * throttle.Value;
                currentEffectiveTorque = currentInitialTorque - engineInternalFriction;
                engineEffectiveTorque = currentInitialTorque - engineInternalFriction;

                acceleration = (engineEffectiveTorque - loadTorque) / engineInertia;
                angularVelocityDelta = acceleration * dt;
                engineAngularVelocity = Mathf.Clamp(engineAngularVelocity + angularVelocityDelta, idleRPM * RPMtoRadians, redlineRPM * RPMtoRadians);

                engineRPM = engineAngularVelocity * RadianstoRPM;                
            }
            #endregion engine

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Engine;
            public void Shutdown()
            {

            }

            public void Start()
            {
                this.throttle = config.throttleVariable;
                this.torqueCurve = config.torqueCurve;
                //this.idleRPM = config.idleRPM;
                this.idleRPM = 900f;
                //this.redlineRPM = config.redlineRPM;
                this.redlineRPM = 7500f;
            }

            public void Update(float dt)
            {
                Update(dt,0f);
            }
            #endregion IVehicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {
                
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"ENGINE");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  throttle : {throttle.Value.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  engineEffectiveTorque: {engineEffectiveTorque.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  engineAngularVelocity: {engineAngularVelocity.ToString("F3")}");
                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  RPM: {engineRPM.ToString("F3")}");
                return yOffset;
            }
            #endregion IDebugInformation


        }

    }
}