using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
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
            
            public float radius{ get; private set; } // meter


            // Parameters 
            float gravity => 9.81f;
            float normalForce => wheelData.rb.mass/4f;
            float RollingResistanceCoefficient = 0.0164f; //https://www.engineeringtoolbox.com/rolling-friction-resistance-d_1303.html
            float RollingResistanceforce => normalForce * RollingResistanceCoefficient;



            #region wheelComponent
            public WheelComponent(WheelID id, WheelSO config, WheelHitData wheelHitData)
            {
                this.config = config;
                this.id = id;
                this.radius = config.RadiusMeter;
                this.wheelData = wheelHitData;
                this.wheelData.wheel = this;
            }
            public void UpdatePhysics(float dt, float throttle, float brake)
            {
                if (!wheelData.isGrounded)
                    return;
                float maxTorque = 120f;
                float diffRatio = 3.9f;
                float firstGear = 3.727f;
                float reverseGear = -3.507f;
                var driveTorque = maxTorque * firstGear * diffRatio * throttle;
                var brakeTorque = maxTorque * reverseGear * diffRatio * brake;
                CalculateLongitudinalForce(dt, driveTorque, brakeTorque);
                CalculateLateralForce(dt);

                AddTireForce();
            }


            #region Lateral Forces
            float Fx;
            float pacjekaLat;
            float currentSlipAngleDeg;
            public float lateralSlipRatio => currentSlipAngleDeg/90f;
            void CalculateLateralForce(float dt)
            {
                Fx = 0f;

                var vX = wheelData.velocityLS.x;
                var vZ = wheelData.velocityLS.z;    

                currentSlipAngleDeg = Mathf.Atan(MathHelper.SafeDivide(vX, Mathf.Abs(vZ))) * Mathf.Rad2Deg;
                pacjekaLat = Pacjeka.quickPacjekaLat(currentSlipAngleDeg);
                var lateralClamp = usefullMass * Mathf.Abs(vX) / dt;

                Fx += pacjekaLat * lateralClamp;
            }


            float wheelLoadFactor => normalForce / (normalForce * 4f);
            float usefullMass => wheelLoadFactor * wheelData.rb.mass;
            
            #endregion Lateral Forces

            #region Longitudinal Forces

            public float LongitudinalSlipRatio => currentSlipRatio;
            float currentSlipRatio;
            float Fz;
            
            

            float longitudinalAngularVelocity = default;
            
            float wheelMass => 20f;  // kilogram
            float wheelMomentOfInteria => 0.5f * wheelMass * radius * radius;
            float wheelInvInertia => 1 / wheelMomentOfInteria;
            
            void CalculateLongitudinalForce(float dt, float driveTorque, float brakeTorque)
            {
                if (wheelData.id == WheelID.RightRear)
                {
                    int tt = 1;
                }
                if (wheelData.id == WheelID.RightRear && brakeTorque < 0f)
                {
                    int t = 1;
                }

                

                UnityEngine.Debug.Log($"Drive torque: {driveTorque + brakeTorque}\n VeloZ {wheelData.velocityLS.z}");
                Fz = 0f;
                // current wheel velocity 
                var longitudinalWheelSpeedMS = wheelData.velocityLS.z;
                var angularVelocity = MathHelper.SafeDivide(longitudinalWheelSpeedMS, radius) ; // rad / s

                if (wheelData.id == WheelID.RightRear && brakeTorque < 0f && wheelData.velocityLS.z < 8f)
                {
                    int t = 1;
                }



                // new wheel velocity after applying drive forces 
                var totalNetTorque = driveTorque + brakeTorque; // Newton torque
                var fwdForce = totalNetTorque * radius; // Newton
                var longitudinalAngularAcceleration = totalNetTorque / wheelInvInertia; // rad/s²
                var newAngularVelocity = angularVelocity + longitudinalAngularAcceleration * dt;// rad/s

                // work whether wheel angular velocity is reliable for its sign direction
                bool isWheelStopped = Mathf.Abs(angularVelocity) < float.Epsilon;
                                
                float slideSign = isWheelStopped ? MathHelper.Sign(longitudinalWheelSpeedMS) : MathHelper.Sign(newAngularVelocity);
                

                // slip speed = (new speed - current speed) * slipDirection
                float wheelSlipSpeedMS = ((newAngularVelocity * radius) - longitudinalWheelSpeedMS) * slideSign;

                // slip ratio
                var wheelSlipRatioSAE = wheelSlipSpeedMS / Mathf.Abs(longitudinalWheelSpeedMS);
                currentSlipRatio = wheelSlipRatioSAE;

                // longitudinal friction clamp, maximum force that will stop this slide 
                var wheelTorque = totalNetTorque;
                var maxFwdForce = (usefullMass * wheelSlipSpeedMS / dt) + (wheelTorque * slideSign / radius);

                // angular clamp 
                var estimatedLongitudinalWheelSpeedMS = longitudinalWheelSpeedMS + (fwdForce * slideSign * dt / wheelData.body.mass);
                
                var estimatedNewRoadSpinVelocity = estimatedLongitudinalWheelSpeedMS / radius;  //RADS/s
                var spinVelDiff = angularVelocity - estimatedNewRoadSpinVelocity; // Rads/s
                var spinFriction = (spinVelDiff / wheelInvInertia * dt); // 
                float spinMaxGroundFwdforce = spinFriction * slideSign / radius;

                var longitudinalFrictionClamp = LongitudinalFrictionClamp(dt, wheelSlipSpeedMS, wheelTorque, slideSign);


                var dtFz = MathHelper.Sign(currentSlipRatio) * Mathf.Abs(totalNetTorque)  / radius;
                if (Mathf.Abs(dtFz) > Mathf.Abs(longitudinalFrictionClamp))
                    dtFz = longitudinalFrictionClamp;

                Fz += dtFz;

                // rolling resistance 
                //if (Mathf.Abs(wheelData.velocityLS.z) > float.Epsilon)
                //{
                //    var resistanceSign = -MathHelper.Sign(wheelData.velocityLS.z);
                //    Fz += resistanceSign * RollingResistanceforce;
                //}
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
                // forward - longitudinal force                 
                FzForceVec = Vector3.ProjectOnPlane(wheelData.forward, wheelData.hitInfo.normal) * Fz;

                // sideways - lateral force 
                FxForceVec = Vector3.ProjectOnPlane(wheelData.right, wheelData.hitInfo.normal) * Fx;

                // add force;
                wheelData.rb.AddForceAtPosition(FzForceVec + FxForceVec, wheelData.axlePosition);
                UnityEngine.Debug.DrawRay(wheelData.axlePosition, FzForceVec,Color.blue);
                UnityEngine.Debug.DrawRay(wheelData.axlePosition, FxForceVec, Color.red);
            }
          

            public void Update(float dt, float throttle, float brake)
            {
                UpdatePhysics(dt, throttle, brake );
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
                UpdatePhysics(dt, 0f,0f);
            }

            #endregion IVehicleComponent



            #region IDebugInformation
            public void DrawGizmos()
            {
                Gizmos.color = Color.red;

                Gizmos.color = Color.blue;

            }
            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"WHEEL: {this.wheelData.id.ToString()}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" km/h : {(this.wheelData.SpeedKMH).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  m/s : {(this.wheelData.SpeedMS).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Vz : {(this.wheelData.velocityLS.z).ToString("f5")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Vx : {(this.wheelData.velocityLS.x).ToString("f5")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fz : {(this.Fz).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" FzVec : {(this.FzForceVec).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" AngularVelo: {(this.longitudinalAngularVelocity).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" slipRatio: {(this.currentSlipRatio).ToString("f3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" intertia: {(this.wheelMomentOfInteria).ToString("f3")}");

                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Fx : {(this.Fx).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" PLat: {(this.pacjekaLat).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Angle: {(this.currentSlipAngleDeg).ToString("f1")}");
                return yOffset;
            }

            #endregion IDebugInformation


            public class WheelLateralForcesData
            {
                public float Vx = default; // sideways motion - meters 
                public float Vz = default; // forward motion - meters 


                // relaxation length
                // the speed at which the tire
                public float relaxationLength = 0.01f;


                // Slip Ratio 
                // -1 slipping to the left 
                //  1 slipping to the right 
                //  0 no slip
                public float SlipX; 

            }
        }
    }
}