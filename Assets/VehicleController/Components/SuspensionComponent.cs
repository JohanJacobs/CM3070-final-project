
using System;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
	namespace VehicleComponent
	{
		public class SuspensionComponent:IVehicleComponent, IDebugInformation
        {
            public WheelID id { get; private set; }
            SuspensionSO config;
            Transform mountPoint;
            WheelComponent wheelComponent;

            float restLength; // meters;
            float raycastLength=> restLength + wheelComponent?.radius??0f;
            float currentLength;
            float previousLength;

            [SerializeField]
            float springStrength;
            [SerializeField]
            float damperStrength;

            WheelHitData hitData;

            [Header("Debug Info")]
            float springCompression = 0f;
            public SuspensionComponent(SuspensionSO config, WheelComponent  wheel, Transform mountPoint, Rigidbody rb)
            {
                this.config = config;
                this.mountPoint = mountPoint;
                this.id = wheel.id;
                this.wheelComponent = wheel;

                this.restLength = config.RestLengthMeter;
                var springLengthInMillimeter = config.RestLengthMeter * 100f;
                this.springStrength = config.suspensionForce;                
                this.damperStrength = config.DamperStrength;
                
                this.hitData = new WheelHitData();
                this.hitData.rb = rb;
                wheel.SetWheelHitData(this.hitData);

                this.currentLength = this.restLength;
                this.previousLength = this.restLength;
            }

            private void UpdatePhysics(float dt)
            {

                Ray ray = new Ray(mountPoint.position, -mountPoint.up);
                

                if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastLength))
                {                    
                    currentLength = hitInfo.distance - wheelComponent.radius; // in meters

                    springCompression = 1f - (currentLength / restLength);

                    var compressedLength = restLength - currentLength; // the force works per spring distance not compressed
                    var springForce = compressedLength * springStrength;


                    var damperVelocity = (previousLength - currentLength) / dt;
                    var damperForce = damperVelocity * damperStrength;
                    var normalForce = springForce + damperForce;
                    var forceVector = normalForce * 100f * hitInfo.normal;

                    hitData.rb.AddForceAtPosition(forceVector, hitInfo.point);
                    previousLength = currentLength;

                    this.hitData.distanceToWheelAxle = currentLength;
                    this.hitData.isGrounded = true;
                    this.hitData.normalForce = normalForce;
                    this.hitData.hitPoint = hitInfo.point;
                    this.hitData.hitNormal = hitInfo.normal;
                }
                else
                {
                    Debug.DrawRay(mountPoint.position, -mountPoint.up * raycastLength, Color.red);
                    previousLength = currentLength;
                    currentLength = restLength;                    
                    this.hitData.distanceToWheelAxle = restLength;
                    this.hitData.isGrounded = false;
                    this.hitData.normalForce = 0f;
                    this.hitData.hitPoint = Vector3.zero;
                    this.hitData.hitNormal = Vector3.zero;
                    springCompression = 0f;
                }
            }
            #region IVehicleComponent
            public ComponentTypes GetComponentType()
            {
                return ComponentTypes.Suspension;
            }

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

            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep),$"Susp : {id.ToString()}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" Compression: {(springCompression * 100f).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" springStrength: {(springStrength).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" normalForce: {(this.hitData.normalForce).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" restLength: {(this.restLength).ToString("f1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $" currentLength: {(this.currentLength).ToString("f1")}");

                return yOffset;
            }

            #endregion IDebugInformation
        }
    }
}