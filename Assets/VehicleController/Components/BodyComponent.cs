using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class BodyComponent : IVehicleComponent 
        {
            BodySO config;
            FloatVariable steerInput;

            Transform leftWheel;
            Transform rightWheel;

            float wheelBaseLength;
            float turnRadius;
            float wheelBaseRearTrackLength;

            float ackermanAngleLeft = 0f;
            float ackermanAngleRight = 0f;

            public BodyComponent( BodySO config, Transform leftWheel, Transform rightWheel)
            {
                this.config = config;
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
                // calculate angle in degrees 

                float steerValue = steerInput.Value;

                
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

                leftWheel.localRotation = Quaternion.Euler(new Vector3(leftWheel.localRotation.x, ackermanAngleLeft, leftWheel.localRotation.z));
                rightWheel.localRotation = Quaternion.Euler(new Vector3(rightWheel.localRotation.x, ackermanAngleLeft, rightWheel.localRotation.z));
            }

            #endregion bodycomponent
        }
    }
}