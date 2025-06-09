using System.Diagnostics;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class WheelComponent : IVehicleComponent, IDebugInformation
        {

            WheelSO config;
            WheelHitData wheelHitData;
            public WheelID id { get; private set; }
            
            public float radius{ get; private set; } // meter

            private float wheelRelaxationLength = 0.01f;
            private float frictionCoefficient = 1.2f; // mu
            public WheelComponent(WheelSO config, WheelID id)
            {
                this.config = config;
                this.id = id;
                this.radius = config.RadiusMeter;                
            }

            public void SetWheelHitData(WheelHitData wheelHitData)
            {
                this.wheelHitData = wheelHitData;
            }

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
            public void Update(float dt, float driveTorque)
            {
                
                GetSx(driveTorque);
                GetSy(dt);
                AddTireForce();
            }

            public void Update(float dt)
            {
                Update(dt, 0f);
            }
            #endregion IVehicleComponent

            #region wheelComponent

            void GetSx(float drivetorque)
            {
                wheelHitData.longitudinalSlip = Mathf.Clamp(drivetorque,-1f,1f);
            }

            void GetSy(float dt)
            {
                //GetLateratialSlip

                var steadyState = Mathf.Sign(wheelHitData.linearVelocityLS.x/-1f);

                var coeff = Mathf.Abs(wheelHitData.linearVelocityLS.x / wheelRelaxationLength) * dt;

                var lateralSlip = wheelHitData.lateralSlip + (steadyState - wheelHitData.lateralSlip) * coeff;
                wheelHitData.lateralSlip = Mathf.Clamp(lateralSlip, -1f, 1f);
            }


            Vector3 lateralForce;
            Vector3 longitudinalForce;

            void AddTireForce()
            {
                var maxforce = Mathf.Max(wheelHitData.normalForce, 0f);
                
                // lateral - side
                var lateralForceStrength = wheelHitData.lateralSlip * maxforce;
                
                lateralForce = Vector3.ProjectOnPlane(wheelHitData.mountPoint.right, wheelHitData.hitNormal).normalized * lateralForceStrength * frictionCoefficient;

                // longitudinal - forward
                var longitudinalForceStrength = wheelHitData.longitudinalSlip * maxforce;
                longitudinalForce = Vector3.ProjectOnPlane(wheelHitData.mountPoint.forward, wheelHitData.hitNormal).normalized * longitudinalForceStrength * frictionCoefficient;

                var totalForce = (lateralForce + longitudinalForce) * 100f;
                wheelHitData.rb.AddForceAtPosition(totalForce, wheelHitData.mountPoint.position - wheelHitData.mountPoint.up * wheelHitData.distanceToWheelAxle);
            }
            #endregion wheelComponent

            #region IDebugInformation
            public void DrawGizmos()
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(this.wheelHitData.hitPoint, lateralForce.normalized);
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(this.wheelHitData.hitPoint, longitudinalForce.normalized);                                
            }
            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                return yOffset;
            }

            #endregion IDebugInformation
        }
    }
}