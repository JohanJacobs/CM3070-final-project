using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {

        public class RollbarComponet : IVehicleComponent,IDebugInformation
        {
            Rigidbody rb;

            public float rollbarStrength = 5000f;

            WheelHitData leftWheel;
            WheelHitData rightWheel;

            #region RollbarComponent
            public RollbarComponet(Rigidbody rb,WheelHitData leftWheelData, WheelHitData rightWheelData)
            {
                this.rb = rb;
                this.leftWheel = leftWheelData;  
                this.rightWheel = rightWheelData;    
            }

            Vector3 leftForce = default;
            Vector3 rightForce = default;
            void UpdatePhysics(float dt)
            {
                // distance traveled
                var leftCompression = leftWheel.isGrounded ? leftWheel.springCompression : 0f * 100f;
                var rightCompression = rightWheel.isGrounded ? rightWheel.springCompression : 0f * 100f;

                var antiRollforce = (leftCompression - rightCompression) * rollbarStrength;

                if (leftWheel.isGrounded) 
                {
                    leftForce = rb.transform.up * antiRollforce;
                    rb.AddForceAtPosition(leftForce, leftWheel.axlePosition);
                }
                else
                {
                    leftForce = Vector3.zero;
                }

                if (rightWheel.isGrounded)
                {
                    rightForce = rb.transform.up * -antiRollforce;
                    rb.AddForceAtPosition(rightForce, rightWheel.axlePosition);
                }
                else
                {
                    rightForce = Vector3.zero;
                }
            }

            #endregion RollbarComponent

            #region IVerhicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.AnitRollbar;
            public void Shutdown()
            {

            }

            public void Start()
            {

            }

            public void Update(float dt)
            {
                UpdatePhysics(dt);
            }
            #endregion IVerhicleComponent


            #region IDebugInformation
            public void DrawGizmos()
            {
                var drawlines = false;
                // rollbar connection
                if (drawlines)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(leftWheel.axlePosition, rightWheel.axlePosition);

                    var offset = rb.transform.right *0.5f;
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawLine(leftWheel.axlePosition +offset, leftWheel.axlePosition  + offset+leftForce);
                    Gizmos.DrawLine(rightWheel.axlePosition-offset, rightWheel.axlePosition - offset+ rightForce);
                }
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {

                return yOffset;
            }

            #endregion IDebugInformation
        }

    }
}