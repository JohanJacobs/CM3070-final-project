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
        public interface IEngineRPM
        {
            public float RedlineRPM{ get; }
            public float IdleRRM { get; }
            public float CurrentRPM { get; }

        }

        public class EngineComponent : IVehicleComponent<EngineComponentStepParams>,IDebugInformation, IEngineRPM
        {
            EngineSO config;
            FloatVariable throttleInput;
            

            FloatVariable idleRPM;      // Revolutions per minute
            FloatVariable redlineRPM;   // Revolutions per minute
            FloatVariable currentRPM;   // Revolutions per minute
            FloatVariable startFriction;            // nm
            FloatVariable frictionCoefficient;       // mu
            FloatVariable engineInertia;            // kg m²
            BoolVariable tcEnabled;

            RevLimiter revLimiter;

            public float RedlineRPM => redlineRPM.Value;
            public float IdleRRM => idleRPM.Value;
            public float CurrentRPM => currentRPM.Value;

            float RPMtoRad (float rpm)=> PhysicsHelper.Conversions.RPMToRad(rpm);
            float RadtoRPM(float radians) => PhysicsHelper.Conversions.RadToRPM(radians);

            public EngineComponent(EngineSO config,VehicleVariablesSO variables)
            {
                this.config = config;

                // setup variables
                this.throttleInput = variables.throttle;

                this.idleRPM = variables.engineIdleRPM;
                this.redlineRPM = variables.engineRedlineRPM;
                this.currentRPM = variables.engineCurrentRPM;
                this.startFriction = variables.engineStartFriction;
                this.frictionCoefficient = variables.engineInternalFrictionCoefficient;
                this.engineInertia = variables.engineInertia;
                this.tcEnabled = variables.TractionControlEnabled;

                this.revLimiter = new();
            }

            #region Engine Component

            AnimationCurve torqueCurve;
            float maxEffectiveTorque => torqueCurve.Evaluate(engineRPM);
            float engineRPM => currentRPM.Value;


            float engineInternalFriction => startFriction.Value + (engineRPM * frictionCoefficient.Value);
            
            float autoThrottle => (throttleValue < 1f)?0.01f:0f; // idling, only add it if the throttle is not maxed out
            float tractionControlFactor = 1f;

            bool isTCEnabled => tcEnabled.Value;
            
            float throttleValue => throttleInput.Value * tractionControlFactor;
            float currentInitialTorque => (maxEffectiveTorque + engineInternalFriction) * (throttleValue + autoThrottle) * revLimiter.ThrottleFactor;
            float currentEffectiveTorque => currentInitialTorque - engineInternalFriction;
            float idleAngularRotation => RPMtoRad(idleRPM.Value);
            float redlineAngularRotation => RPMtoRad(redlineRPM.Value);

            public float engineAngularVelocity=100f;
            float engineEffectiveTorque =default;
            
            void UpdateEngineAcceleration(float dt,float loadTorque,float tractionControlThrottleAdjustFactor)
            {
                tractionControlFactor = isTCEnabled?tractionControlThrottleAdjustFactor:1f;

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
                revLimiter.Step(parameters.dt, this);

                UpdateEngineAcceleration(parameters.dt, parameters.loadTorque, parameters.tc.TractionControlThrottleAdjustFactor);
            }
            #endregion IVehicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {
                
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"ENGINE");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Throttle : {throttleInput.Value.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Torque: {engineEffectiveTorque.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  RPM: {engineRPM.ToString("F3")}");
                return yOffset;
            }
            #endregion IDebugInformation

            #region VehicleComponentStepParams


        }
        public class EngineComponentStepParams
        {
            public EngineComponentStepParams(float dt, float loadTorque, ITractionControl tc)
            {
                this.dt = dt;
                this.loadTorque = loadTorque;
                this.tc = tc;
            }
            public float dt;
            public float loadTorque;
            public ITractionControl tc;
        }
        #endregion  VehicleComponentStepParams

    }
}