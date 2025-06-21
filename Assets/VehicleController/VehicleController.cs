using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;
using static vc.VehicleController;
/*
    handbrake
    audio
    visuals for skidding
    visuals for smoke // spinning
 */
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
        [SerializeField] VehicleConfiguration.WheelConfigData[] wheelConfig;
        [SerializeField,Space] VehicleConfiguration.SuspensionConfigData[] suspensionConfig;
        [SerializeField] DifferentialSO differentialConfig;
        [SerializeField] TransmissionSO transmissionConfig;
        [SerializeField] EngineSO engineConfig;
        [SerializeField] ClutchSO clutchConfig;
        [SerializeField] BodySO bodyConfig;

        Rigidbody carRigidbody;

        Vehicle vehicle;
        public void Awake()
        {
            carRigidbody = GetComponent<Rigidbody>();
            if (carRigidbody == null)
            {
                Debug.LogError("Missing rigid body!");
            }

            vehicle = Vehicle.Setup(carRigidbody, wheelConfig, suspensionConfig, bodyConfig, differentialConfig, transmissionConfig, clutchConfig, engineConfig);
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

            // DRIVE PHASE 
            var diffTorque = vehicle.transmission.CaclulateDifferentialTorque(vehicle.clutch.clutchTorque);
            var driveTorque = vehicle.differential.CalculateWheelOutputTorque(diffTorque);

            var brakeBalance = 0.65f; // % to front
            var maxBrakeTorque = 2000f; // passanger car 2000nm - 10000 nm  break force
            var brakeTorqueFront = brakeInput.Value * maxBrakeTorque * brakeBalance;
            var brakeTorqueRear = Mathf.Min(brakeInput.Value * maxBrakeTorque * (1f-brakeBalance) + handbrakeInput.Value * maxBrakeTorque, maxBrakeTorque);


            // front wheel 
            vehicle.wheels[WheelID.LeftFront ].Update(dt,0f,brakeTorqueFront);
            vehicle.wheels[WheelID.RightFront].Update(dt,0f,brakeTorqueFront);

            // rear wheels
            vehicle.wheels[WheelID.LeftRear ].Update(dt, driveTorque[0], brakeTorqueRear);
            vehicle.wheels[WheelID.RightRear].Update(dt, driveTorque[1], brakeTorqueRear);


            // FEEDBACK PHASE
            var transVelo = vehicle.differential.CalculateTransmissionVelocity(vehicle.wheels[WheelID.LeftRear].wheelAngularVelocity, vehicle.wheels[WheelID.RightRear].wheelAngularVelocity);
            var clutchVelo = vehicle.transmission.CalculateClutchVelocity(transVelo);

            vehicle.clutch.Update(clutchVelo, vehicle.transmission.GearRatio, vehicle.engine.engineAngularVelocity);
            vehicle.engine.Update(dt, vehicle.clutch.clutchTorque);
        }

        #region Vehicle
                
        private void OnDestroy()
        {
            vehicle.body.Shutdown();
            vehicle.engine.Shutdown();
            vehicle.clutch.Shutdown();
            vehicle.transmission.Shutdown();
            vehicle.differential.Shutdown();
            vehicle.rollbarFront.Shutdown();
            vehicle.rollbarRear.Shutdown();
            vehicle.wheels.ForEach(w => w.Value.Shutdown());
            vehicle.suspension.ForEach(s => s.Value.Shutdown());
        }

        #endregion Vehicle

        #region DebugInformation

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            vehicle.rollbarFront.DrawGizmos();

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
            yOffset = vehicle.engine.OnGUI(xPos, yOffset, yStep);
            yOffset = vehicle.clutch.OnGUI(xPos, yOffset, yStep);
            yOffset = vehicle.transmission.OnGUI(xPos, yOffset, yStep);
            yOffset = vehicle.differential.OnGUI(xPos, yOffset, yStep);

            //yOffset = vehicle.suspension[WheelID.LeftFront ].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.RightFront].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.LeftRear  ].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.RightRear ].OnGUI(xPos, yOffset, yStep);

            //yOffset = vehicle.wheels[WheelID.LeftFront ].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.wheels[WheelID.RightFront].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.wheels[WheelID.LeftRear  ].OnGUI(xPos, yOffset, yStep);
            yOffset = vehicle.wheels[WheelID.RightRear ].OnGUI(xPos, yOffset, yStep);

        }
        #endregion
    }

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

        public static Vehicle Setup(Rigidbody carRigidbody, 
            VehicleConfiguration.WheelConfigData[] wheelConfig, 
            VehicleConfiguration.SuspensionConfigData[] suspensionConfig,
            BodySO bodyConfig,
            DifferentialSO differentialConfig,
            TransmissionSO transmissionConfig,
            ClutchSO clutchConfig,
            EngineSO engineConfig
            )
        {
            Vehicle newVehicle = new();

            var wheelHitData = WheelHitData.SetupDefault(carRigidbody);

            // wheels 
            newVehicle.wheels = new();
            foreach (var wc in wheelConfig)
            {
                newVehicle.wheels.Add(wc.ID, new WheelComponent(wc.ID, wc.Config, wheelHitData[wc.ID]));
            }

            // Setup Suspensions            
            newVehicle.suspension = new();
            foreach (var susp in suspensionConfig)
            {
                newVehicle.suspension.Add(susp.ID, new SuspensionComponent(susp.Config, wheelHitData[susp.ID], susp.mountPoint));
            }

            // car body
            newVehicle.body = new BodyComponent(bodyConfig, carRigidbody, newVehicle.suspension[WheelID.LeftFront].mountPoint, newVehicle.suspension[WheelID.RightFront].mountPoint);
            newVehicle.body.Start();

            wheelHitData.ForEach(whd=>whd.Value.body = newVehicle.body);


            newVehicle.suspension.ForEach(s=>s.Value.Start());             
            newVehicle.wheels.ForEach(w=>w.Value.Start());

            newVehicle.rollbarFront = new(carRigidbody, wheelHitData[WheelID.LeftFront], wheelHitData[WheelID.RightFront]);
            newVehicle.rollbarFront.Start();

            newVehicle.rollbarRear = new(carRigidbody, wheelHitData[WheelID.LeftRear], wheelHitData[WheelID.RightRear]);
            newVehicle.rollbarRear.Start();

            newVehicle.differential = new(differentialConfig, 2);
            newVehicle.differential.Start();

            newVehicle.transmission = new(transmissionConfig);
            newVehicle.transmission.Start();

            newVehicle.clutch = new ClutchComponent(clutchConfig);
            newVehicle.clutch.Start();

            newVehicle.engine = new EngineComponent(engineConfig);
            newVehicle.engine.Start();

            return newVehicle;
        }
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