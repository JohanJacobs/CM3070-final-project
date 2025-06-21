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

            float bodyDragCoefficient = 0.34f; // constant 

            //http://www.mayfco.com/dragcd~1.htm
            float bodyArea = 1.74f; // m² 

            float wheelBaseLength; // m
            float turnRadius;      // m
            float wheelBaseRearTrackLength; // m

            float aeroDrag => 0f; // TODO:
            float bodyDrag => MathHelper.CalculateDrag(velocityLS.z,bodyArea,bodyDragCoefficient); // nm TODO:
            public float DragForce => aeroDrag + bodyDrag;
            public float mass => 1500; // kg
            public float MStoKMH = 3.6f; //constant
            public float SpeedKMH =>velocityLS.z * MStoKMH; // Km/H
            Vector3 velocityWS => rb.velocity; // MS
            Vector3 velocityLS => rb.transform.InverseTransformDirection(velocityWS); //MS


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
                AddBodyDragForce(dt);
            }
            #endregion IVehicleComponent

            #region bodycomponent
            void UpdateAckermanSteering()
            {
                float steerValue = steerInput.Value;
                float ackermanAngleLeft = 0f;
                float ackermanAngleRight = 0f;

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

            void AddBodyDragForce(float dt)
            {
                rb.AddForce(-rb.transform.forward * bodyDrag);
            }
            #endregion bodycomponent

            #region IDebugInformation
            public void DrawGizmos()
            {
                
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"- BODY:");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Km/h :{SpeedKMH.ToString("F0")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Velo :{velocityLS.ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  B.Drag :{bodyDrag.ToString("f2")}");


                return yOffset;
            }
            #endregion IDebugInformation
        }
    }
}