using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
    namespace VehicleComponent
    {
        public class EngineComponent : IVehicleComponent<EngineComponentStepParams>,IDebugInformation
        {
            EngineSO config;
            FloatVariable throttle;
            
            AnimationCurve torqueCurve;

            FloatVariable idleRPM;      // Revolutions per minute
            FloatVariable redlineRPM;   // Revolutions per minute
            FloatVariable currentRPM;   // Revolutions per minute
            FloatVariable startFriction;            // nm
            FloatVariable frictionCoefficient;       // mu
            FloatVariable engineInertia;            // kg m²

            float RPMtoRad (float rpm)=> PhysicsHelper.Conversions.RPMToRad(rpm);
            float RadtoRPM(float radians) => PhysicsHelper.Conversions.RadToRPM(radians);

            public EngineComponent(EngineSO config,VehicleVariablesSO variables)
            {
                this.config = config;

                // setup variables
                this.throttle = variables.throttle;

                this.idleRPM = variables.engineIdleRPM;
                this.redlineRPM = variables.engineRedlineRPM;
                this.currentRPM = variables.engineCurrentRPM;
                this.startFriction = variables.engineStartFriction;
                this.frictionCoefficient = variables.engineInternalFrictionCoefficient;
                this.engineInertia = variables.engineInertia;
            }

            #region Engine Component

            float maxEffectiveTorque => torqueCurve.Evaluate(engineRPM);
            float engineRPM => currentRPM.Value;
            
            float engineInternalFriction => startFriction.Value + (engineRPM * frictionCoefficient.Value);
            float currentInitialTorque => (maxEffectiveTorque + engineInternalFriction) * throttle.Value;
            float currentEffectiveTorque => currentInitialTorque - engineInternalFriction;
            float idleAngularRotation => RPMtoRad(idleRPM.Value);
            float redlineAngularRotation => RPMtoRad(redlineRPM.Value);

            public float engineAngularVelocity=100f;
            float engineEffectiveTorque =default;
            
            void UpdateEngineAcceleration(float dt,float loadTorque)
            {
                engineEffectiveTorque = currentEffectiveTorque;
                float acceleration = (engineEffectiveTorque - loadTorque) / engineInertia.Value;                
                engineAngularVelocity = Mathf.Clamp(engineAngularVelocity + acceleration * dt, idleAngularRotation , redlineAngularRotation);
                currentRPM.Value = RadtoRPM(engineAngularVelocity);

            }
            #endregion Engine Component

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Engine;
            public void Start()
            {                
                this.torqueCurve = config.torqueCurve;
                this.idleRPM.Value = this.config.idleRPM;                
                this.redlineRPM.Value = this.config.redlineRPM;
                this.startFriction.Value = this.config.startFriction;
                this.engineInertia.Value = this.config.engineEnirtia;
                this.frictionCoefficient.Value = this.config.frictionCoefficient;
            }

            public void Shutdown()
            {

            }
            public void Step(EngineComponentStepParams parameters)
            {
                UpdateEngineAcceleration(parameters.dt, parameters.loadTorque);
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

            #region VehicleComponentStepParams


        }
        public class EngineComponentStepParams
        {
            public EngineComponentStepParams(float dt, float loadTorque)
            {
                this.dt = dt;
                this.loadTorque = loadTorque;
            }
            public float dt;
            public float loadTorque;
        }
        #endregion  VehicleComponentStepParams

    }
}