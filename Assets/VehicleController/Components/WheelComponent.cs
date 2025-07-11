using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
    namespace VehicleComponent
    {
        public class WheelComponent : IVehicleComponent<WheelComponenetStepParameters>, IDebugInformation, IHasInertia
        {

            WheelSO config;
            WheelHitData wheelData;
            Transform wheelMesh;

            ScriptableObjectVariable Tires;
            public WheelID id { get; private set; }
            public float radius { get; private set; }        // meter
            public float wheelAngularVelocity { get; private set; } //rads
            public float WheelMeshDegRotation => PhysicsHelper.Conversions.RadtoDeg(wheelAngularVelocity);

            // Parameters 
            public float wheelMass { get; private set; }    //KG
            public float GetInertia => wheelInertia;
            public float Fn => Mathf.Max(wheelData.normalforce, 0f) * wheelFrictionCoefficient; // down force on top of the wheel
            public float Fz = default; // nm force applied in the forward direction
            public float Fx = default; // nm force applied in sideways direction
            public bool isGrounded => wheelData.isGrounded;

            float wheelInertia => PhysicsHelper.InertiaWheel(wheelMass, radius); //kg m²

            // Friction
            float rollingResistanceCoefficient => wheelFrictionValues[wheelData.FrictionSurfaceType].RR;
            float wheelFrictionCoefficient => wheelFrictionValues[wheelData.FrictionSurfaceType].Dry;
            Dictionary<IFrictionSurface.SurfaceType, WheelSurfaceProperties> wheelFrictionValues;


            float LongitudinalRelaxationLength = 0.005f;

            float dt = default;
            float driveTorque = default;
            float brakeTorque = default;
            bool isLocked = true;
            //Vector2 combinedSlip = default; // combined Slip X=Force, Y=sideways

            #region wheelComponent

            public WheelComponent(WheelID id, WheelSO config, WheelHitData wheelHitData, Transform wheelMesh, VehicleVariablesSO variables)
            {
                this.config = config;
                this.id = id;
                this.Tires = variables.Tires;

                this.wheelData = wheelHitData;
                this.wheelData.wheel = this;
                
                this.wheelMesh = wheelMesh;
            }

            public void UpdatePhysics(float dt, float driveTorque = default, float brakeTorque = default)
            {
                this.dt = dt;
                this.driveTorque = driveTorque;
                this.brakeTorque = brakeTorque;

                if (!wheelData.isGrounded)
                    return;

                CalculateLongitudinal();
                CalculateLateral();
                CombinedSlip();

                ApplyWheelForces();
            }
            public void UpdateWheelData(ScriptableObjectBase soBase)
            {
                var wd = this.Tires.Value as WheelSO;

                this.wheelFrictionValues = WheelSurfaceProperties.CreateDictionary(wd.frictionProperties);
                this.wheelMass = wd.Mass; //kg
            }

            public static event UnityAction<WheelID, WheelHitData> onVisualWheelUpdate;
            public void UpdateVisuals(float dt)
            {
                // update position
                wheelMesh.transform.position = wheelData.axlePosition;

                float xRot = (WheelMeshDegRotation) % 45f;
                
                float yRot = wheelData.suspensionMountPoint.eulerAngles.y;
                                
                wheelMesh.Rotate(new Vector3(WheelMeshDegRotation * dt, 0f, 0f), Space.Self);

                // notify listeners that the wheel visuals and data has been udpated.
                onVisualWheelUpdate?.Invoke(id, wheelData);
            }
            void CombinedSlip()
            {
                var combined = new Vector2(slipZ, latCalc.lateralSlipRatio);
                wheelData.combinedSlip = (combined.magnitude > 1.0f) ? combined.normalized : combined;
            }
            #region Lateral Forces
            
            
            private WheelLateralSlipCalculator latCalc = new();
            
            public float SlipAngleDynamic => latCalc.lateralSlipAngleDynamic;
            
            public float SlipAngleRatio => latCalc.lateralSlipAngle;
            public float LateralSlipAngle => latCalc.lateralSlipAngle;
            public float lateralSlipVelocity => wheelData.velocityLS.x; // m/s


            void CalculateLateral()
            {
                //currentSlipAngleDeg = Mathf.Atan(MathHelper.SafeDivide(wheelData.velocityLS.x, Mathf.Abs(wheelData.velocityLS.z))) * Mathf.Rad2Deg;

                latCalc.CalculateSlip(wheelData.velocityLS, dt);
            }
                        
            #endregion Lateral Forces

            #region Longitudinal Forces

            public float LongitudinalSlipRatio => slipZ;
            float slipZ = default;

            public float longSlipVelocity = default; // m/s

            // Longitudinal Slip calculations
            void CalculateLongitudinal()
            {
                UpdateWheelVelocity();
                CalculateSlipZ();                
            }

            #region Longitudinal Wheel Acceleration 
            //wheel Acceleration Calculations
            float frictionTorque => Fz * radius; // nm
            float angularAcceleration => MathHelper.SafeDivide((driveTorque - frictionTorque), wheelInertia); // Rad/s²
            void UpdateWheelVelocity()
            {
                wheelAngularVelocity += angularAcceleration * dt; // Rad/s
 
                float rollResistanceTorque = wheelData.normalforce * radius * rollingResistanceCoefficient; // nm
                float totalBrakingTorque= -Mathf.Sign(wheelAngularVelocity) * (brakeTorque + rollResistanceTorque); // nm
                float brakeAcceleration = (totalBrakingTorque / wheelInertia) * dt;
                float newAngularVelocity = wheelAngularVelocity + brakeAcceleration;
                if (Mathf.Sign(newAngularVelocity) != Mathf.Sign(wheelAngularVelocity))
                {
                    wheelAngularVelocity = 0f;
                    isLocked = true;
                }
                else
                {
                    wheelAngularVelocity = newAngularVelocity;
                    isLocked = false;
                }

                // speed at which we slide
                longSlipVelocity = wheelAngularVelocity * radius - wheelData.velocityLS.z;
            }
            #endregion Longitudinal Wheel Acceleration 
           
            #region Slip Ratio Calculations
            
            float targetAngularVelocity => (wheelData.velocityLS.z / radius); // Rad/s
            float targetAngularAccelleration => (wheelAngularVelocity - targetAngularVelocity) / dt; // Rad/s²
            float targetTorque => targetAngularAccelleration * wheelInertia; //Rad/s
            float maximumFrictionTorque => wheelData.normalforce * radius * wheelFrictionCoefficient; //nm
            
            float lockedWheelSlipZ => MathHelper.Sign(longSlipVelocity);
            
            float rollingWheelSlipZ => Mathf.Clamp(MathHelper.SafeDivide(targetTorque, maximumFrictionTorque), -100f, 100f);  //clamp to maximum slip value
            float slipBasedOnWheelState => isLocked ? lockedWheelSlipZ : rollingWheelSlipZ;
            float longSlipVelocityRelaxationCoeff => Mathf.Clamp((Mathf.Abs(longSlipVelocity) / LongitudinalRelaxationLength) * dt, 0f, 1f);
            void CalculateSlipZ()
            {   
                slipZ += (slipBasedOnWheelState - slipZ) * longSlipVelocityRelaxationCoeff;                
            }
            #endregion Slip Ratio Calculations
            
            #endregion Longitudinal Forces

            Vector3 FzForceVec, FxForceVec;
            void ApplyWheelForces()
            {
                Fz = wheelData.combinedSlip.x * Fn; // Pacejka longitudinal * normalForce;
                FzForceVec = Vector3.ProjectOnPlane(wheelData.forward, wheelData.hitInfo.normal).normalized * Fz;

                Fx = wheelData.combinedSlip.y * Fn; //Pacejka Lateral * normalForce;
                FxForceVec = Vector3.ProjectOnPlane(wheelData.right, wheelData.hitInfo.normal).normalized * Fx;

                // add force to rigidbody
                wheelData.rb.AddForceAtPosition((FzForceVec + FxForceVec), wheelData.axlePosition);                                
            }

            #endregion wheelComponent

            #region IVehicleComponent
            public ComponentTypes GetComponentType()
            {
                return ComponentTypes.Wheel;
            }

            public void Start()
            {
                this.wheelMesh.parent = wheelData.suspensionMountPoint;
                this.wheelMesh.gameObject.SetActive(true);

                this.Tires.Value = this.config;
                this.Tires.OnValueChanged += UpdateWheelData;
                this.radius = config.RadiusMeter;
                UpdateWheelData(this.Tires.Value);
            }

            public void Shutdown()
            {
                this.Tires.OnValueChanged -= UpdateWheelData;
            }

            public void Step(WheelComponenetStepParameters parameters)
            {
                UpdatePhysics(parameters.dt, parameters.driveTorque, parameters.brakeTorque);
            }

            #endregion IVehicleComponent


            #region IDebugInformation
            public void DrawGizmos()
            {
                if (!wheelData.isGrounded)
                    return;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(wheelData.axlePosition, wheelData.axlePosition + FxForceVec);
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(wheelData.axlePosition, wheelData.axlePosition + FzForceVec);

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(wheelData.axlePosition, wheelData.forward);
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(wheelData.axlePosition, wheelData.suspensionMountPoint.TransformDirection(wheelData.velocityLS.normalized));

            }
            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"WHEEL {this.wheelData.id.ToString()}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Locked : {(this.isLocked?"Yes":"No")}");

                // Longitudinal 
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" cLongSlip : {(this.wheelData.combinedSlip.x).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fz : {(this.Fz).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" DrTrq: {(this.driveTorque).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" AngularVelo: {(this.wheelAngularVelocity).ToString("f2")}");

                // Lateral                 
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" lateralSlipAngle: {this.latCalc.lateralSlipAngle.ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" cLatSlip: {(this.wheelData.combinedSlip.y).ToString("f5")}");                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fx: {(this.Fx).ToString("f2")}");

                // Friction surface 
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Surface: {(wheelData.FrictionSurfaceName)}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Mu: {(this.wheelFrictionCoefficient).ToString("F2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" rr: {(this.rollingResistanceCoefficient).ToString("F2")}");
                return yOffset;
            }
            
            #endregion IDebugInformation

            public class WheelLateralSlipCalculator
            {

                // Slip Ratio 
                // -1 slipping to the left 
                //  1 slipping to the right 
                //  0 no slip

                public float lateralSlipRatio { get; private set; }
                public float lateralSlipAngleDynamic = default;

                // Relaxation Length
                // the distance a tire travels before its lateral (sideways) force reaches a steady-state value
                // after a sudden change in slip angle (the angle between the direction the tire is pointing and
                // the direction it is actually traveling). It describes the delay between when a slip angle is
                // introduced and when the cornering force reaches its steady-state value
                //  Relaxations lengths have been found to be between 0.12 and 0.45 meters, with higher values
                //  corresponding to higher velocities and heavier loads - https://en.wikipedia.org/wiki/Relaxation_length
                public float relaxationLength = 0.001f;

                // Slip Angle 
                // angle between the direction in which a wheel is pointing and the direction in which it is actually traveling
                // This slip angle results in a force, the cornering force, which is in the plane of the contact patch and
                // perpendicular to the intersection of the contact patch and the midplane of the wheel.[1] This cornering force
                // increases approximately linearly for the first few degrees of slip angle, then increases non-linearly to a
                // maximum before beginning to decrease. (Pacejka, Hans B. (2006). Tire and Vehicle Dynamics (Second ed.). Society of Automotive Engineers. pp. 3, 612. ISBN 0-7680-1702-5.)
                // https://en.wikipedia.org/wiki/Slip_angle
                public float lateralSlipAngle { get; private set; }

                public float lateralSlipAnglePeak = 8f; /// peak slip angle for maximum grip (8 degrees) for dynamic slip calculation

                private float RelaxationCoefficient(Vector3 veloLS, float dt) => Mathf.Clamp((Mathf.Abs(veloLS.x) / relaxationLength) * dt, 0f, 1f);

                private float HighSpeedSteadyState(Vector3 veloLS) 
                {
                    // slip angle
                    lateralSlipAngle = Mathf.Atan(MathHelper.SafeDivide(-veloLS.x, Mathf.Abs(veloLS.z)))*Mathf.Rad2Deg;
                    return lateralSlipAngle;
                }
                private float SteadyStateTransition(Vector3 veloLS) => MathHelper.MapAndClamp(veloLS.magnitude, 3f, 6f, 0f, 1f);
                private float LowSpeedSteadyState(Vector3 veloLS) => lateralSlipAnglePeak * MathHelper.Sign(-1f*veloLS.x);
                private float CaclulateDynamicSlipAngle(Vector3 veloLS, float dt)
                {
                    var steadyState = Mathf.Lerp(LowSpeedSteadyState(veloLS), HighSpeedSteadyState(veloLS), SteadyStateTransition(veloLS));
                    var newSlipAngleDynamic = lateralSlipAngleDynamic + (steadyState - lateralSlipAngleDynamic) * RelaxationCoefficient(veloLS, dt);
                    lateralSlipAngleDynamic = Mathf.Clamp(newSlipAngleDynamic, -90f, 90f);

                    return lateralSlipAngleDynamic;
                }
                public float CalculateSlip(Vector3 veloLS, float dt)
                {
                    lateralSlipAngleDynamic = CaclulateDynamicSlipAngle(veloLS,dt);
                    lateralSlipRatio = Mathf.Clamp(MathHelper.SafeDivide(lateralSlipAngleDynamic,lateralSlipAnglePeak), -1f, 1f);
                    //slipRatio = Mathf.Abs(slipRatio) < float.Epsilon ? 0f : slipRatio;
                    // TODO: add a logic to always nudge slip ratio to 0 if its very close to zero
                    return lateralSlipRatio;
                }
            }
        }
        #region Wheel Componenet Step Parameters
        public class WheelComponenetStepParameters 
        {
            public WheelComponenetStepParameters(float dt, float driveTorque, float brakeTorque)
            {
                this.dt = dt;
                this.driveTorque = driveTorque;
                this.brakeTorque = brakeTorque;
            }
            public float dt;
            public float driveTorque;
            public float brakeTorque;
        }
        #endregion Wheel Componenet Step Parameters
    }
}