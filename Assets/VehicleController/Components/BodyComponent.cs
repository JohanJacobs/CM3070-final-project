using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class BodyComponent : IVehicleComponent , IDebugInformation
        {
            Rigidbody rb;
            BodySO config;
            FloatVariable steerInput;

            Transform leftWheel;
            Transform rightWheel;

            float wheelBaseLength;
            float turnRadius;
            float wheelBaseRearTrackLength;

            float ackermanAngleLeft = 0f;
            float ackermanAngleRight = 0f;


            float aeroDrag => 0f; // TODO:
            float bodyDrag => 0f; // TODO:
            public float DragForce => aeroDrag + bodyDrag;
            public float mass => 1500; // kg



            public BodyComponent( BodySO config, Rigidbody rb, Transform leftWheel, Transform rightWheel)
            {
                this.config = config;
                this.rb = rb;
                this.leftWheel = leftWheel;
                this.rightWheel = rightWheel;
                this.steerInput = config.steerVariable;

                wheelBaseLength = config.wheelBaseLength;
                turnRadius = config.turnRadius;
                wheelBaseRearTrackLength = config.wheelBaseRearTrackLength;
            }

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Body;

            public void Start()
            {
                
            }

            public void Shutdown()
            {
                
            }

            public void Update(float dt)
            {
                UpdateAckermanSteering();
            }
            #endregion IVehicleComponent

            #region bodycomponent
            void UpdateAckermanSteering()
            {

                float steerValue = steerInput.Value;

                
                // calculate angle in degrees 
                if (steerValue < 0f)
                {
                    ackermanAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius - (wheelBaseRearTrackLength / 2))) * steerValue;
                    ackermanAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius + (wheelBaseRearTrackLength / 2))) * steerValue;
                }
                else if (steerValue > 0f)
                {
                    ackermanAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius + (wheelBaseRearTrackLength / 2))) * steerValue;
                    ackermanAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBaseLength / (turnRadius - (wheelBaseRearTrackLength / 2))) * steerValue;
                }

                // Rotate the steering geometry
                leftWheel.localRotation = Quaternion.Euler(new Vector3(leftWheel.localRotation.x, ackermanAngleLeft, leftWheel.localRotation.z));
                rightWheel.localRotation = Quaternion.Euler(new Vector3(rightWheel.localRotation.x, ackermanAngleLeft, rightWheel.localRotation.z));
            }

            public void DrawGizmos()
            {
                
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"- BODY:");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Velo :{rb.transform.InverseTransformDirection(rb.velocity)}");

                return yOffset;
            }
            #endregion bodycomponent
        }
    }
}