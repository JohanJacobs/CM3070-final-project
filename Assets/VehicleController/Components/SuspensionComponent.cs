
using System;
using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
	namespace VehicleComponent
	{
		public class SuspensionComponent:IVehicleComponent, IDebugInformation
        {
            SuspensionSO config;
            public Transform mountPoint { get; private set; }

            float restLength; // meters;
            float raycastLength=> restLength + wheelData.wheel?.radius??0f;

            float currentLength;
            float previousLength;
            float springCompression => 1f - (currentLength / restLength);
            float compressedLength => restLength - currentLength;
            float wheelRadius => wheelData.wheel.radius;


            public float SuspensionCompressionRatio => (currentLength/restLength);

            float normalForce; 
            public bool isGrounded { get; private set; }
            public Vector3 axlePosition => mountPoint.position - mountPoint.up * currentLength;
                        
            float springStrength;            
            float damperStrength;

            WheelHitData wheelData;            
            
            #region SuspensionComponent
            public SuspensionComponent(SuspensionSO config, WheelHitData wd, Transform mountPoint)
            {                
                this.config = config;
                this.wheelData = wd;
                this.mountPoint = mountPoint;

                this.currentLength = this.restLength;
                this.previousLength = this.restLength;
                this.restLength = config.RestLengthMeter;                
                this.springStrength = config.suspensionForce;                
                this.damperStrength = config.DamperStrength;

                this.wheelData.suspension = this;
            }

            private void UpdatePhysics(float dt)
            {

                Ray ray = new Ray(mountPoint.position, -mountPoint.up);

                if (Physics.Raycast(ray, out wheelData.hitInfo, raycastLength))
                {                    
                    currentLength = wheelData.hitInfo.distance - wheelRadius; // in meters                   

                    var springForce = compressedLength * springStrength;
                    var damperVelocity = (previousLength - currentLength) / dt;
                    var damperForce = damperVelocity * damperStrength;
                    normalForce = springForce + damperForce;
                    var forceVector = normalForce * 100f * wheelData.hitInfo.normal;

                    wheelData.rb.AddForceAtPosition(forceVector, mountPoint.position);
                    previousLength = currentLength;
                    isGrounded = true;

                    // update wheel data 
                    var worldVelo = this.wheelData.rb.GetPointVelocity(wheelData.hitInfo.point);
                    this.wheelData.velocityLS = mountPoint.InverseTransformDirection(worldVelo);

                }
                else
                {
                    wheelData.hitInfo = new();
                    previousLength = currentLength;
                    currentLength = restLength;
                    isGrounded = false;
                }


            }


            #endregion SuspensionComponent
           
            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Suspension;

            public void Start()
            {

            }

            public void Shutdown()
            {   
                
            }

            public void Update(float dt)
            {
                UpdatePhysics(dt);
            }

            #endregion IVehicleComponent


            #region IDebugInformation
            public void DrawGizmos()
            {
                // Draw the contact point 
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(wheelData.hitInfo.point, 0.01f); // hit point
                Gizmos.DrawSphere(wheelData.axlePosition, 0.01f); // axle position
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep),$"SUSP : {this.wheelData.id.ToString()}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" currentLength: {(this.currentLength).ToString("f1")}");                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" restLength: {(this.restLength).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" normalForce: {(this.normalForce).ToString("f1")}");

                return yOffset;
            }

            #endregion IDebugInformation
        }
    }
}