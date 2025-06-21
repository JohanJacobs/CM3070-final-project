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
            public float wheelMass { get; private set; }    //KG
            public float wheelAngularVelocity { get; private set; }
            // Parameters 
            float gravity => 9.81f;
            //float normalForce => wheelData.normalforce;//wheelData.rb.mass/4f;
            float RollingResistanceCoefficient = 0.0164f; //https://www.engineeringtoolbox.com/rolling-friction-resistance-d_1303.html
            float WheelFrictionCoefficient = 1.0f;
            float RollingResistanceforce => wheelData.normalforce * RollingResistanceCoefficient;

            float driveTorque;
            

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
            public void UpdatePhysics(float dt, float driveTorque)
            {
                if (!wheelData.isGrounded)
                    return;
                
                if (id == WheelID.RightRear)
                {
                    int tttt = 1;
                }    
                this.driveTorque = driveTorque;                
                CalculateLongitudinalForce(dt, driveTorque);
                CalculateLateralForce(dt);

                AddTireForce();
            }

            #region Lateral Forces
            float slipX;
            float pacjekaLat = default;
            float currentSlipAngleDeg =default;
            float lateralSlipRatio => currentSlipAngleDeg / 90f;
            private WheelLateralSlipCalculator latCalc = new();

                       
            void CalculateLateralForce(float dt)
            {
                
                currentSlipAngleDeg = Mathf.Atan(MathHelper.SafeDivide(wheelData.velocityLS.x, Mathf.Abs(wheelData.velocityLS.z))) * Mathf.Rad2Deg;

                var lateralSlipRatio = latCalc.CalculateSlip(wheelData.velocityLS, dt);
                slipX =  lateralSlipRatio;
            }

            float wheelLoadFactor => wheelData.normalforce / (wheelData.normalforce * 4f);
            float usefullMass => wheelLoadFactor * wheelData.rb.mass;
            
            #endregion Lateral Forces

            #region Longitudinal Forces

            public float LongitudinalSlipRatio => slipZ;
            //float slipRatioZ = default;
            float slipZ = default;
            float Fz = default;

            //private WheelLongitudinalSlipCalculator longCalc =new();
            

            void CalculateLongitudinalForce(float dt, float driveTorque)
            {
                //slipRatioZ = longCalc.CaclulateSlipRatio(id, t, Fz, wheelData.normalforce, wheelData.velocityLS, driveTorque, wheelMass, radius, WheelFrictionCoefficient);                
                //Sz = Mathf.Max(wheelData.normalforce, 0f) * slipRatioZ ;                
                if (id == WheelID.RightRear)
                {
                    int tttt = 1;
                }

                WheelAcceleration(dt, driveTorque);
                CalculateSlipZ(dt,driveTorque);

                //velocity 
            }
            float _targetAngularVelo;
            float _angularAccel;
            float _targetTorque;
            float _friction;
            void CalculateSlipZ(float dt, float driveTorque)
            {
                if (Mathf.Abs(wheelData.normalforce) < float.Epsilon)
                {
                    slipZ = 0f;
                    return;
                }
                if (id == WheelID.RightRear && driveTorque > 0f)
                {
                    UnityEngine.Debug.Log($"drive torque {driveTorque}");
                }
                _targetAngularVelo = (wheelData.velocityLS.z / radius);
                _angularAccel  = (wheelAngularVelocity - _targetAngularVelo) / dt;
                var _wheelInertia = 1.5f;/* 0.5f * wheelMass * radius * radius;*/
                _targetTorque = _angularAccel * _wheelInertia;
                _friction = (wheelData.normalforce) * radius * WheelFrictionCoefficient;
                slipZ = Mathf.Clamp(MathHelper.SafeDivide(_targetTorque,_friction), -1f, 1f);
            }

            void WheelAcceleration(float dt, float drivetorque)
            {
                var frictionTorque = (Fz) * radius;
                var wheelIntertia = 1.5f;
                var angularAcceleration = MathHelper.SafeDivide((drivetorque - frictionTorque), wheelIntertia);
                wheelAngularVelocity = wheelAngularVelocity + angularAcceleration * dt;
            }


            float LongitudinalFrictionClamp(float dt,float wheelSlipSpeedMS, float wheelTorque, float slideSign)
            {
                var maxForwardForce = (usefullMass * wheelSlipSpeedMS / dt) + (wheelTorque * slideSign / radius);
                return maxForwardForce;
            }
            
            #endregion Longitudinal Forces

            Vector3 FzForceVec, FxForceVec;
            void AddTireForce()
            {
                if (id == WheelID.RightRear)
                {
                    int tttt = 1;
                }
                // forward - longitudinal force                 

                var force = Mathf.Max(wheelData.normalforce, 0f);
                FzForceVec = Vector3.ProjectOnPlane(wheelData.forward, wheelData.hitInfo.normal).normalized * slipZ * force;
                Fz = slipZ * force ;

                // sideways - lateral force 
                FxForceVec = Vector3.ProjectOnPlane(wheelData.right, wheelData.hitInfo.normal).normalized * slipX * force;
                
                // add force;
                wheelData.rb.AddForceAtPosition((FzForceVec + FxForceVec), wheelData.axlePosition);                                
            }
          

            public void Update(float dt, float driveTorque)
            {
                UpdatePhysics(dt, driveTorque);
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
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" km/h : {(this.wheelData.SpeedKMH).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  m/s : {(this.wheelData.SpeedMS).ToString("f3")}");                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Vz : {(this.wheelData.velocityLS.z).ToString("f5")}");
                //GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Vx : {(this.wheelData.velocityLS.x).ToString("f5")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fy : {(this.wheelData.normalforce).ToString("f5")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"--------------");

                // Longitudinal 
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fz : {(this.Fz).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" r : {(this.radius).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" DrTrq: {(this.driveTorque).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" inertia: {(1.5f).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" AngularVelo: {(this.wheelAngularVelocity).ToString("f3")}");

                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" SLIPCALC: ");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" _targetAngularVelo: {(this._targetAngularVelo).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" _angularAccel: {(this._angularAccel).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" _targetTorque: {(this._targetTorque).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" _friction: {(this._friction).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" slip: {(MathHelper.SafeDivide(this._targetTorque,this._friction)).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" slip: {(Mathf.Clamp(MathHelper.SafeDivide(this._targetTorque, this._friction),-1f,1f)).ToString("f3")}");


                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" FzVec : {(this.FzForceVec).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" slipZ : {(this.slipZ).ToString("f1")}");
                //GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" slipRatio: {(this.slipRatioZ).ToString("f3")}");

                // lateral 
                
                //GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Plat: {(this.pacjekaLat).ToString("f1")}");
                //GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" SA: {(this.lsc.slipAngle).ToString("f4")}");
                //GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" SADyn: {(this.lsc.slipAngleDynamic).ToString("f4")}");
                //GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" SR: {(this.lsc.slipRatio).ToString("f4")}");


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