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
            float RPMtoRad (float rpm)=> PhysicsHelper.Conversions.RPMToRad(rpm);
            float RadtoRPM(float radians) => PhysicsHelper.Conversions.RadToRPM(radians);

            public EngineComponent(EngineSO config)
            {
                this.config = config;
                
                // setup variables

            }

            #region Engine Component
            public void Update(float dt, float loadTorque)
            {
                UpdateEngineAcceleration(dt, loadTorque);
            }

            float startFriction = 50f;          // nm
            float frictionCoefficient = 0.02f;  // mu
            float engineInertia = 0.2f;         // kg m²

            float maxEffectiveTorque => torqueCurve.Evaluate(engineRPM);
            float engineRPM => RadtoRPM(engineAngularVelocity);
            float engineInternalFriction => startFriction + (engineRPM * frictionCoefficient);
            float currentInitialTorque => (maxEffectiveTorque + engineInternalFriction) * throttle.Value;
            float currentEffectiveTorque => currentInitialTorque - engineInternalFriction;
            float idleAngularRotation => RPMtoRad(idleRPM);
            float redlineAngularRotation => RPMtoRad(redlineRPM);

            public float engineAngularVelocity=100f;
            float engineEffectiveTorque =default;
            
            void UpdateEngineAcceleration(float dt,float loadTorque)
            {
                engineEffectiveTorque = currentEffectiveTorque;
                float acceleration = (engineEffectiveTorque - loadTorque) / engineInertia;                
                engineAngularVelocity = Mathf.Clamp(engineAngularVelocity + acceleration * dt, idleAngularRotation , redlineAngularRotation);
            }
            #endregion Engine Component

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
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Throttle : {throttle.Value.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Torque: {engineEffectiveTorque.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  RPM: {engineRPM.ToString("F3")}");
                return yOffset;
            }
            #endregion IDebugInformation


        }

    }
}