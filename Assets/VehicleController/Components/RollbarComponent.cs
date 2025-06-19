using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {

        public class RollbarComponet : IVehicleComponent
        {
            Rigidbody rb;

            float rollbarStrength => 5000f;

            WheelHitData leftWheel;
            WheelHitData rightWheel;

            #region RollbarComponent
            public RollbarComponet(Rigidbody rb,WheelHitData leftWheelData, WheelHitData rightWheelData)
            {
                this.rb = rb;
                this.leftWheel = leftWheelData;  
                this.rightWheel = rightWheelData;    
            }

            void UpdatePhysics(float dt)
            {
                // distance traveled
                var leftCompression = leftWheel.isGrounded ? leftWheel.SuspensionCompressionRatio : 1f;
                var rightCompression = rightWheel.isGrounded ? rightWheel.SuspensionCompressionRatio : 1f;

                var antiRollforce = (leftCompression - rightCompression) * rollbarStrength;

                if (leftWheel.isGrounded) 
                {
                    rb.AddForceAtPosition(rb.transform.up * -antiRollforce, leftWheel.axlePosition);
                }

                if (rightWheel.isGrounded)
                {
                    rb.AddForceAtPosition(rb.transform.up * antiRollforce, rightWheel.axlePosition);
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
        }

    }
}