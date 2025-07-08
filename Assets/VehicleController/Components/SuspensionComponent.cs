
using System;
using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
	namespace VehicleComponent
	{
		public class SuspensionComponent:IVehicleComponent<SuspensionComponentStepParams>, IDebugInformation
        {
            SuspensionSO config;
            public Transform mountPoint { get; private set; }

            public FloatVariable restLength; // meters;
            public FloatVariable springStrength; // newton meters      
            public FloatVariable damperStrength; // newton meters

            float raycastLength=> restLength.Value + wheelData.wheel?.radius??0f;

            float currentLength;
            float previousLength;
            public float springCompression => (compressedLength / restLength.Value);
            float compressedLength => restLength.Value - currentLength; // m
            float wheelRadius => wheelData.wheel.radius; // m
            float damperVelocity (float dt) => (previousLength - currentLength) / dt;


            public float normalForce; 
            public bool isGrounded { get; private set; }
            public Vector3 axlePosition => mountPoint.position - mountPoint.up * currentLength;
                        
            Vector3 forceVector;

            WheelHitData wheelData;            
            
            #region SuspensionComponent
            public SuspensionComponent(SuspensionSO config, WheelHitData wd, Transform mountPoint, VehicleVariablesSO variables)
            {                
                this.config = config;
                this.wheelData = wd;
                this.mountPoint = mountPoint;

;               this.restLength = (wd.id == WheelID.LeftFront || wd.id == WheelID.RightFront) ? variables.RestLengthFront : variables.RestLengthRear;
                this.springStrength = (wd.id == WheelID.LeftFront || wd.id == WheelID.RightFront) ? variables.SpringForceFront : variables.SpringForceRear;
                this.damperStrength = (wd.id == WheelID.LeftFront || wd.id == WheelID.RightFront) ? variables.DamperForceFront : variables.DamperForceRear;

                this.wheelData.suspension = this;
            }

            private void UpdatePhysics(float dt)
            {
                //  Calculate the direction of the ray
                Ray ray = new Ray(mountPoint.position, -mountPoint.up);

                // Raycast length is : rest length = (wheelRadius + Suspension Rest length)
                if (Physics.Raycast(ray, out wheelData.hitInfo, raycastLength))
                {
                    UpdateFrictionSurface(wheelData.hitInfo.transform);

                    currentLength = wheelData.hitInfo.distance - wheelRadius; // in meters

                    // calculate suspension forces using Hooks Law Fz = -kFw
                    var springForce = (compressedLength * springStrength.Value);
                    var damperForce = damperVelocity(dt) * damperStrength.Value;
                                        
                    normalForce = (springForce + damperForce) * 100f; // scale to the correct factor
                    forceVector = normalForce * wheelData.hitInfo.normal;

                    // add force to the body                    
                    wheelData.rb.AddForceAtPosition(forceVector, axlePosition);
                    previousLength = currentLength;
                    isGrounded = true;
                    
                    // update the wheel velocity based on moving objects below it 
                    var myVelo = this.wheelData.rb.GetPointVelocity(mountPoint.position);
                    var objectBelowWheelVelo = Vector3.zero;
                    if (wheelData.hitInfo.rigidbody)
                    {
                        objectBelowWheelVelo = wheelData.hitInfo.rigidbody.GetPointVelocity(this.wheelData.hitInfo.point);
                    }

                    var worldVelo = myVelo - objectBelowWheelVelo;

                    // Update local Velocity
                    this.wheelData.velocityLS = mountPoint.InverseTransformDirection(worldVelo);
                    this.wheelData.velocityLS.y = 0f;

                }
                else
                {
                    wheelData.hitInfo = new();
                    previousLength = currentLength;
                    currentLength = restLength.Value;
                    isGrounded = false;
                    normalForce = 0f;
                    forceVector = Vector3.zero;
                }


            }

            private void UpdateFrictionSurface(Transform hitObject)
            {
                if(hitObject.TryGetComponent<IFrictionSurface>(out var surface))
                {
                    wheelData.surface = surface;
                }
                else
                {
                    wheelData.surface = IFrictionSurface.CreateDefault();
                }
            }
            #endregion SuspensionComponent
           
            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Suspension;

            public void Start()
            {
                this.restLength.Value = config.RestLengthMeter;

                this.currentLength = this.restLength.Value;
                this.previousLength = this.restLength.Value;

                this.springStrength.Value = config.suspensionForce;
                this.damperStrength.Value = config.DamperStrength;
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
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" length / rest: {(this.currentLength).ToString("f2")}/{(this.restLength.Value).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" compression: {(this.springCompression).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" normalForce: {(this.normalForce).ToString("f1")}");

                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" wheelRadius: {(this.wheelData.wheel.radius).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" rayDist: {(this.wheelData.hitInfo.distance).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" currentL: {(currentLength).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" prevL: {(previousLength).ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" sStrength: {(springStrength).Value.ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" dStrength: {(damperStrength).Value.ToString("f1")}");


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