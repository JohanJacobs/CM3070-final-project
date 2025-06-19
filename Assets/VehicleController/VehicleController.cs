using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;

namespace vc
{
    public class VehicleController : MonoBehaviour
    {
        [Title("Variables")]
        [SerializeField] FloatVariable steerInput;
        [SerializeField] FloatVariable throttleInput;
        [SerializeField] FloatVariable brakeInput;
        [SerializeField] FloatVariable handbrakeInput;

        [Title("Configuration")]
        [SerializeField]
        VehicleConfiguration.WheelConfigData[] wheelConfig;
        [SerializeField,Space]
        VehicleConfiguration.SuspensionConfigData[] suspensionConfig;
        [SerializeField]
        DifferentialSO differentialConfig;
        [SerializeField]
        TransmissionSO transmissionConfig;
        [SerializeField]
        EngineSO EngineConfig;
        [SerializeField]
        ClutchSO ClutchConfig;
        [SerializeField]
        BodySO bodyConfig;


        Rigidbody carRigidbody;
        
        public void Awake()
        {
            carRigidbody = GetComponent<Rigidbody>();
            if (carRigidbody == null)
            {
                Debug.LogError("Missing rigid body!");
            }

            SetupVehicle();         
        }

        public void Update()
        {            
        }
        
        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            float throttle = throttleInput.Value;
            float brake = brakeInput.Value;
            vehicle.body.Update(dt);

            
            vehicle.suspension[WheelID.LeftFront].Update(dt);
            vehicle.suspension[WheelID.RightFront].Update(dt);
            vehicle.suspension[WheelID.LeftRear].Update(dt);
            vehicle.suspension[WheelID.RightRear].Update(dt);

            vehicle.rollbarFront.Update(dt);
            vehicle.rollbarRear.Update(dt);

            vehicle.wheels[WheelID.LeftFront].Update(dt, throttle, brake);
            vehicle.wheels[WheelID.RightFront].Update(dt, throttle, brake);

            vehicle.wheels[WheelID.LeftRear].Update(dt, throttle, brake);
            vehicle.wheels[WheelID.RightRear].Update(dt, throttle, brake);

        }

        #region Vehicle
        public class Vehicle
        {
            public Dictionary<WheelID, SuspensionComponent> suspension;
            public Dictionary<WheelID, WheelComponent> wheels;
            public DifferentialComponent differential;
            public TransmissionComponent transmission;
            public EngineComponent engine;
            public ClutchComponent clutch;
            public BodyComponent body;
            public RollbarComponet rollbarFront, rollbarRear;
        }
        Vehicle vehicle;

        private void SetupVehicle()
        {
            vehicle = new();


            var wheelHitData = WheelHitData.SetupDefault(carRigidbody);
            
            // wheels 
            vehicle.wheels = new();
            foreach (var wc in wheelConfig)
            {
                vehicle.wheels.Add(wc.ID, new WheelComponent(wc.ID, wc.Config, wheelHitData[wc.ID]));
            }

            // Setup Suspensions            
            vehicle.suspension = new();
            foreach (var susp in suspensionConfig)
            {
                vehicle.suspension.Add(susp.ID, new SuspensionComponent(susp.Config, wheelHitData[susp.ID], susp.mountPoint));
            }

            // car body
            vehicle.body = new BodyComponent(bodyConfig, vehicle.suspension[WheelID.LeftFront].mountPoint, vehicle.suspension[WheelID.RightFront].mountPoint);                       
            foreach (var whd in wheelHitData)
            {
                whd.Value.body = vehicle.body;
            }

            vehicle.rollbarFront = new(carRigidbody, wheelHitData[WheelID.LeftFront], wheelHitData[WheelID.RightFront]);
            vehicle.rollbarRear = new(carRigidbody, wheelHitData[WheelID.LeftRear], wheelHitData[WheelID.RightRear]);

        }
        #endregion Vehicle


        #region DebugInformation

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            vehicle.wheels[WheelID.RightFront].DrawGizmos();
            vehicle.wheels[WheelID.LeftFront].DrawGizmos();
            vehicle.wheels[WheelID.RightRear].DrawGizmos();
            vehicle.wheels[WheelID.LeftRear].DrawGizmos();


            vehicle.suspension[WheelID.RightFront].DrawGizmos();
            vehicle.suspension[WheelID.LeftFront].DrawGizmos();
            vehicle.suspension[WheelID.RightRear].DrawGizmos();
            vehicle.suspension[WheelID.LeftRear].DrawGizmos();

        }

        private void OnGUI()
        {
            float yOffset = 10f;
            float yStep = 20f;
            float xPos = 10f;

            yOffset = vehicle.body.OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.LeftFront ].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.RightFront].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.LeftRear  ].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.RightRear ].OnGUI(xPos, yOffset, yStep);

            //yOffset = vehicle.wheels[WheelID.LeftFront ].OnGUI(xPos, yOffset, yStep);
            yOffset = vehicle.wheels[WheelID.RightFront].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.wheels[WheelID.LeftRear  ].OnGUI(xPos, yOffset, yStep);
            yOffset = vehicle.wheels[WheelID.RightRear ].OnGUI(xPos, yOffset, yStep);

        }
        #endregion
    }

    namespace VehicleConfiguration
    {
        [System.Serializable]
        public class WheelConfigData
        {            
            [HorizontalGroup("Config")]
            public WheelID ID;
            [HorizontalGroup("Config")]
            public WheelSO Config;

            [HorizontalGroup("Physical")]
            public Transform mesh; 
        }

        [System.Serializable]
        public class SuspensionConfigData
        {
            [HorizontalGroup("Config")]
            public WheelID ID;
            [HorizontalGroup("Config")]
            public SuspensionSO Config;

            [HorizontalGroup("Physical")]
            public Transform mountPoint;
        }
    }
}