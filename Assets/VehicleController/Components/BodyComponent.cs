using System;
using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;
using static vc.VehicleComponent.BodyComponent;

namespace vc
{
    namespace VehicleComponent
    {
        public class BodyComponent : IVehicleComponent<BodyComponentStepParams>, IDebugInformation
        {
            Rigidbody rb;
            BodySO config;

            FloatVariable steerInput;
            FloatVariable speed;
            Transform leftWheel;
            Transform rightWheel;

            GForce gForce = new();

            FloatVariable BodyDragCoefficient; // constant https://www.netcarshow.com/ford/2009-fiesta_econetic/#:~:text=This%20is%20because%20Fiesta%20boasts,performance%2C%20especially%20in%20highway%20cruising.

            //http://www.mayfco.com/dragcd~1.htm
            float bodyArea = 1.74f; // m² 

            float wheelBaseLength; // m
            float turnRadius;      // m
            float wheelBaseRearTrackLength; // m
                                    
            float bodyDrag => PhysicsHelper.CalculateDrag(Mathf.Abs(velocityLS.z), bodyArea, BodyDragCoefficient.Value); // nm TODO: check if drag is always calculated correctly forward and reverse
            public FloatVariable BodyMass { get; private set; } // kg
            public float SpeedKMH => PhysicsHelper.Conversions.MStoKMH(velocityLS.z); // Km/H
            Vector3 velocityLS =default; //MS

            public BodyComponent( BodySO config, Rigidbody rb, Transform leftWheel, Transform rightWheel, VehicleVariablesSO variables)
            {
                this.config = config;
                this.rb = rb;

                // setup variables
                this.steerInput = variables.steer;
                this.speed = variables.speedKMH;

                this.leftWheel = leftWheel;
                this.rightWheel = rightWheel;

                this.BodyMass = variables.BodyMass;
                this.BodyDragCoefficient = variables.BodyDrag;
                
                wheelBaseLength = config.wheelBaseLength;
                turnRadius = config.turnRadius;
                wheelBaseRearTrackLength = config.wheelBaseRearTrackLength;
            }
            
            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Body;

            public void Start()
            {
                speed.Value = 0f;                

                this.rb.mass = this.config.mass;
                this.BodyMass.Value = this.config.mass;
                this.BodyMass.OnValueChanged += BodyMass_OnValueChanged;

                this.BodyDragCoefficient.Value = this.config.coefficientOfDrag;
            }

            public void Shutdown()
            {
                this.BodyMass.OnValueChanged -= BodyMass_OnValueChanged;
            }
            
            public void Step(BodyComponentStepParams parameters)
            {
                this.velocityLS = parameters.velocityLS;
                gForce.updateG(this.velocityLS,parameters.dt);
                UpdateAckermanSteering();
                AddBodyDragForce();
                speed.Value = Mathf.Max(SpeedKMH,0f);
            }
            #endregion IVehicleComponent

            #region bodycomponent
            float RadtoDeg(float radians) => PhysicsHelper.Conversions.RadtoDeg(radians);
            void UpdateAckermanSteering()
            {
                float steerValue = steerInput.Value;
                float ackermanAngleLeft = 0f;
                float ackermanAngleRight = 0f;

                // calculate angle in degrees 
                if (steerValue < 0f)
                {
                    ackermanAngleLeft = RadtoDeg(Mathf.Atan(wheelBaseLength / (turnRadius - (wheelBaseRearTrackLength / 2)))) * steerValue;
                    ackermanAngleRight = RadtoDeg(Mathf.Atan(wheelBaseLength / (turnRadius + (wheelBaseRearTrackLength / 2)))) * steerValue;
                }
                else if (steerValue > 0f)
                {
                    ackermanAngleLeft = RadtoDeg(Mathf.Atan(wheelBaseLength / (turnRadius + (wheelBaseRearTrackLength / 2)))) * steerValue;
                    ackermanAngleRight = RadtoDeg(Mathf.Atan(wheelBaseLength / (turnRadius - (wheelBaseRearTrackLength / 2)))) * steerValue;
                }

                // Rotate the steering geometry
                leftWheel.localRotation = Quaternion.Euler(new Vector3(leftWheel.localRotation.x, ackermanAngleLeft, leftWheel.localRotation.z));
                rightWheel.localRotation = Quaternion.Euler(new Vector3(rightWheel.localRotation.x, ackermanAngleLeft, rightWheel.localRotation.z));
            }

            void BodyMass_OnValueChanged(float value)
            {
                if (value <= 0f)
                    return;

                rb.mass = value;
            }
            Vector3 dragDirection => -rb.transform.forward;
            void AddBodyDragForce()
            {
                rb.AddForce(dragDirection * bodyDrag);
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
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  long Gs :{gForce.longGForce.ToString("F2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  lat  Gs :{gForce.latGForce.ToString("F2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Velo :{velocityLS.ToString("f2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  B.Drag :{bodyDrag.ToString("f3")}");

                return yOffset;
            }


            #endregion IDebugInformation

            // class to pass parameters to step function
            public class BodyComponentStepParams 
            {
                public BodyComponentStepParams(float dt, Vector3 velocityLS)
                {
                    this.dt = dt;
                    this.velocityLS = velocityLS;
                }
                public float dt;
                public Vector3 velocityLS;
            }
        }
    }
}