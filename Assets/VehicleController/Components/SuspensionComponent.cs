
using System;
using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
	namespace VehicleComponent
	{
		public class SuspensionComponent:IVehicleComponent<SuspensionComponentStepParams>, IDebugInformation
        {
            SuspensionSO config;
            public Transform mountPoint { get; private set; }

            public float restLength; // meters;
            float raycastLength=> restLength + wheelData.wheel?.radius??0f;

            float currentLength;
            float previousLength;
            public float springCompression => (compressedLength / restLength);
            float compressedLength => restLength - currentLength; // m
            float wheelRadius => wheelData.wheel.radius; // m
            float damperVelocity (float dt) => (previousLength - currentLength) / dt;


            public float normalForce; 
            public bool isGrounded { get; private set; }
            public Vector3 axlePosition => mountPoint.position - mountPoint.up * currentLength;
                        
            public float springStrength;            
            public float damperStrength;
            Vector3 forceVector;

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

                    // calculate suspension properties 
                    var springForce = compressedLength * springStrength;                    
                    var damperForce = damperVelocity(dt) * damperStrength;

                    // calculate forces
                    normalForce = (springForce + damperForce) * 100f;
                    forceVector = normalForce * wheelData.hitInfo.normal;

                    // add force
                    wheelData.rb.AddForceAtPosition(forceVector, mountPoint.position);
                    previousLength = currentLength;
                    isGrounded = true;



                    var myVelo = this.wheelData.rb.GetPointVelocity(mountPoint.position);
                    var objectBelowWheelVelo = Vector3.zero;
                    if (wheelData.hitInfo.rigidbody)
                    {
                        objectBelowWheelVelo = wheelData.hitInfo.rigidbody.GetPointVelocity(this.wheelData.hitInfo.point);
                    }

                    var worldVelo = myVelo - objectBelowWheelVelo;

                    // update wheel data 
                    this.wheelData.velocityLS = mountPoint.InverseTransformDirection(worldVelo);
                    this.wheelData.velocityLS.y = 0f;

                }
                else
                {
                    wheelData.hitInfo = new();
                    previousLength = currentLength;
                    currentLength = restLength;
                    isGrounded = false;
                    normalForce = 0f;
                    forceVector = Vector3.zero;
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

            public void Step (SuspensionComponentStepParams parameters)
            {                
                UpdatePhysics(parameters.dt);
            }

            #endregion IVehicleComponent


            #region IDebugInformation
            public void DrawGizmos()
            {
                // Draw the contact point 
                var drawPoints = true;
                if (drawPoints)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(wheelData.hitInfo.point, 0.01f); // hit point
                    Gizmos.color = Color.gray;
                    Gizmos.DrawSphere(wheelData.axlePosition, 0.01f); // axle position
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(mountPoint.position, 0.01f); // suspension mount point
                }

                var drawSupsensionLines = true;
                // suspension length 
                if (drawSupsensionLines)
                {
                    var mountPos = mountPoint.position;
                    var castEndPos= mountPoint.position - mountPoint.up * raycastLength;
                    var suspensionEndPos = mountPoint.position - mountPoint.up * currentLength;
                    var wheelEndPos = suspensionEndPos - mountPoint.up* this.wheelRadius;
                    // raycast length
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(mountPos, castEndPos);

                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(mountPos, suspensionEndPos);

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine( suspensionEndPos,wheelEndPos);
                }
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep),$"SUSP : {this.wheelData.id.ToString()}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" length / rest: {(this.currentLength).ToString("f2")}/{(this.restLength).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" compression: {(this.springCompression).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" normalForce: {(this.normalForce).ToString("f1")}");

                return yOffset;
            }

            #endregion IDebugInformation
        }
        #region Suspension Component Step Params
        public class SuspensionComponentStepParams
        {
            public SuspensionComponentStepParams(float dt)
            {
                this.dt = dt;
            }
            public float dt;
        }
        #endregion Suspension Component Step Params
    }
}