using Microsoft.Win32.SafeHandles;
using Sirenix.OdinInspector.Editor;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class WheelComponent : IVehicleComponent, IDebugInformation
        {

            WheelSO config;
            WheelHitData wheelData;
            public WheelID id { get; private set; }
            
            public float radius{ get; private set; }        // meter
            public float wheelAngularVelocity { get; private set; }
            // Parameters 
            public float wheelMass { get; private set; }    //KG
            float wheelInertia = 1.5f;/* 0.5f * wheelMass * radius * radius;*/ //kg m²
            float rollingResistanceCoefficient = 0.0164f; //https://www.engineeringtoolbox.com/rolling-friction-resistance-d_1303.html
            float wheelFrictionCoefficient = 1.0f;
            float LongitudinalRelaxationLength = 0.005f;

            float dt = default;
            float driveTorque = default;
            float brakeTorque = default;
            bool isLocked = true;
            
            #region wheelComponent
            public WheelComponent(WheelID id, WheelSO config, WheelHitData wheelHitData)
            {
                this.config = config;
                this.id = id;
                this.radius = config.RadiusMeter;
                this.wheelMass = config.Mass;
                this.wheelData = wheelHitData;
                this.wheelData.wheel = this;
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
                ApplyWheelForces();
            }

            #region Lateral Forces
            float slipX;
            private WheelLateralSlipCalculator latCalc = new();
            float Fx;
            void CalculateLateral()
            {
                //currentSlipAngleDeg = Mathf.Atan(MathHelper.SafeDivide(wheelData.velocityLS.x, Mathf.Abs(wheelData.velocityLS.z))) * Mathf.Rad2Deg;

                slipX  = latCalc.CalculateSlip(wheelData.velocityLS, dt);
            }
                        
            #endregion Lateral Forces

            #region Longitudinal Forces

            public float LongitudinalSlipRatio => slipZ;
            float slipZ = default;
            float Fz = default; // nm
            float longSlipVelocity = default;

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
            float rollingWheelSlipZ => Mathf.Clamp(MathHelper.SafeDivide(targetTorque, maximumFrictionTorque), -1f, 1f);
            float slipBaseOnWheelState => isLocked ? lockedWheelSlipZ : rollingWheelSlipZ;
            float longSlipVelocityRelaxationCoeff => Mathf.Clamp((Mathf.Abs(longSlipVelocity) / LongitudinalRelaxationLength) * dt, 0f, 1f);
            void CalculateSlipZ()
            {   
                slipZ += (slipBaseOnWheelState - slipZ) * longSlipVelocityRelaxationCoeff;                
            }
            #endregion Slip Ratio Calculations
            
            #endregion Longitudinal Forces

            Vector3 FzForceVec, FxForceVec;
            void ApplyWheelForces()
            {
                // forward - longitudinal force                 
                var normalForce = Mathf.Max(wheelData.normalforce, 0f);

                Fz = slipZ * normalForce;
                FzForceVec = Vector3.ProjectOnPlane(wheelData.forward, wheelData.hitInfo.normal).normalized * Fz;

                // sideways - lateral force 
                Fx = slipX * normalForce;
                FxForceVec = Vector3.ProjectOnPlane(wheelData.right, wheelData.hitInfo.normal).normalized * Fx;

                // add force to rigidbody
                wheelData.rb.AddForceAtPosition((FzForceVec + FxForceVec), wheelData.axlePosition);                                
            }
          

            public void Update(float dt, float driveTorque, float brakeTorque)
            {
                UpdatePhysics(dt, driveTorque,brakeTorque);
            }
            #endregion wheelComponent

            #region IVehicleComponent
            public ComponentTypes GetComponentType()
            {
                return ComponentTypes.Wheel;
            }

            public void Start()
            {
             
            }

            public void Shutdown()
            {
             
            }
            
            public void Update(float dt)
            {
                UpdatePhysics(dt, 0f);
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
                //GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" km/h : {(this.wheelData.SpeedKMH).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" m/s : {(this.wheelData.SpeedMS).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Locked : {(this.isLocked?"Yes":"No")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fy : {(this.wheelData.normalforce).ToString("f5")}");

                // Longitudinal 
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" LongSlip : {(this.slipZ).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fz : {(this.Fz).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" DrTrq: {(this.driveTorque).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" AngularVelo: {(this.wheelAngularVelocity).ToString("f2")}");

                // lateral                 
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" LatSlip: {(this.slipX).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fx: {(this.Fx).ToString("f2")}");
                return yOffset;
            }
            #endregion IDebugInformation

            public class WheelLateralSlipCalculator
            {

                // Slip Ratio 
                // -1 slipping to the left 
                //  1 slipping to the right 
                //  0 no slip

                public float slipRatio { get; private set; }
                public float slipAngleDynamic = default;

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
                public float slipAngle { get; private set; }

                public float slipAnglePeak = 15f; /// peak slip angle for maximum grip (8 degrees) 

                private float RelaxationCoefficient(Vector3 veloLS, float dt) => Mathf.Clamp((Mathf.Abs(veloLS.x) / relaxationLength) * dt, 0f, 1f);

                //private float SteadyState(Vector3 veloLS) => MathHelper.Sign(veloLS.x / -1f);
                //private float NewSlip(Vector3 veloLS, float dt) => slipRatio + ((SteadyState(veloLS) - slipRatio) * RelaxationCoefficient(veloLS, dt));

                private float HighSpeedSteadyState(Vector3 veloLS) 
                {
                    // slip angle
                    slipAngle = Mathf.Atan(MathHelper.SafeDivide(-veloLS.x, Mathf.Abs(veloLS.z)))*Mathf.Rad2Deg;
                    return slipAngle;
                }
                private float SteadyStateTransition(Vector3 veloLS) => MathHelper.MapAndClamp(veloLS.magnitude, 3f, 6f, 0f, 1f);
                private float LowSpeedSteadyState(Vector3 veloLS) => slipAnglePeak * MathHelper.Sign(-1f*veloLS.x);
                private float CaclulateDynamicSlipAngle(Vector3 veloLS, float dt)
                {
                    var steadyState = Mathf.Lerp(LowSpeedSteadyState(veloLS), HighSpeedSteadyState(veloLS), SteadyStateTransition(veloLS));
                    var newSlipAngleDynamic = slipAngleDynamic + (steadyState - slipAngleDynamic) * RelaxationCoefficient(veloLS, dt);
                    slipAngleDynamic = Mathf.Clamp(newSlipAngleDynamic, -90f, 90f);
                    return slipAngleDynamic;
                }

                public float CalculateSlip(Vector3 veloLS, float dt)
                {
                    slipAngleDynamic = CaclulateDynamicSlipAngle(veloLS,dt);
                    slipRatio = Mathf.Clamp(MathHelper.SafeDivide(slipAngleDynamic,slipAnglePeak), -1f, 1f);
                    //slipRatio = slipRatio < float.Epsilon ? 0f : slipRatio;
                    return slipRatio;
                }
            }
                        
        }
    }
}