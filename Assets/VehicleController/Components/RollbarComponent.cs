using UnityEngine;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
    namespace VehicleComponent
    {

        public class RollbarComponent : IVehicleComponent<RollbarComponentStepParams>,IDebugInformation
        {
            public enum RollbarPosition
            {
                Front,
                Rear
            }

            Rigidbody rb;
            AntiRollbarSO config;

            FloatVariable rollbarStrength;

            WheelHitData leftWheel;
            WheelHitData rightWheel;

            #region RollbarComponent
            public RollbarComponent(Rigidbody rb,AntiRollbarSO config, WheelHitData leftWheelData, WheelHitData rightWheelData, VehicleVariablesSO variables)
            {
                this.rb = rb;
                this.config = config;
                
                this.leftWheel = leftWheelData;  
                this.rightWheel = rightWheelData;

                this.rollbarStrength = config.position == RollbarPosition.Front ? variables.AntiRollbarForceFront : variables.AntiRollbarForceRear;
            }

            Vector3 leftForce = default;
            Vector3 rightForce = default;
            void UpdatePhysics(float dt)
            {
                // distance traveled
                var leftCompression = leftWheel.isGrounded ? leftWheel.springCompression : 0f * 100f;
                var rightCompression = rightWheel.isGrounded ? rightWheel.springCompression : 0f * 100f;

                var antiRollforce = (leftCompression - rightCompression) * rollbarStrength.Value;

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
                this.rollbarStrength.Value = this.config.rollbarStrength;
            }


            public void Step(RollbarComponentStepParams parameters)
            {                
                UpdatePhysics(parameters.dt);
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

        #region Rollbar Component Step Params
        public class RollbarComponentStepParams
        {
            public RollbarComponentStepParams(float dt)
            {
                this.dt = dt;
            }
            public float dt;
        }
        #endregion Rollbar Component Step Params
    }
}