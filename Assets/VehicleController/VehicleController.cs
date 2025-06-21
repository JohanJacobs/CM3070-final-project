using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;
using static vc.VehicleController;

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

        [Header("Tweaks")]
        public float springStrength;
        public float damperStrength;
        public float restLength;
        public float rollbarStrength;


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

            // DRIVE PHASE 
            var diffTorque = vehicle.transmission.CaclulateDifferentialTorque(vehicle.clutch.clutchTorque);
            var driveTorque = vehicle.differential.CalculateWheelOutputTorque(diffTorque);

            // front wheel 
            vehicle.wheels[WheelID.LeftFront ].Update(dt, 0f);
            vehicle.wheels[WheelID.RightFront].Update(dt, 0f);

            // rear wheels
            vehicle.wheels[WheelID.LeftRear ].Update(dt, driveTorque[0]);
            vehicle.wheels[WheelID.RightRear].Update(dt, driveTorque[1]);


            // FEEDBACK PHASE
            var transVelo = vehicle.differential.CalculateTransmissionVelocity(vehicle.wheels[WheelID.LeftRear].wheelAngularVelocity, vehicle.wheels[WheelID.RightRear].wheelAngularVelocity);
            var clutchVelo = vehicle.transmission.CalculateClutchVelocity(transVelo);

            vehicle.clutch.Update(clutchVelo, vehicle.transmission.GearRatio, vehicle.engine.engineAngularVelocity);
            vehicle.engine.Update(dt, vehicle.clutch.clutchTorque);

        }

        #region Vehicle

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
            vehicle.body = new BodyComponent(bodyConfig, carRigidbody, vehicle.suspension[WheelID.LeftFront].mountPoint, vehicle.suspension[WheelID.RightFront].mountPoint);
            vehicle.body.Start();

            foreach (var whd in wheelHitData)
            {
                whd.Value.body = vehicle.body;
            }

            //start suspension            
            foreach (var item in vehicle.suspension)
            {
                item.Value.Start();
            }
            // start wheels 
            foreach (var item in vehicle.wheels)
            {
                item.Value.Start();
            }

            vehicle.rollbarFront = new(carRigidbody, wheelHitData[WheelID.LeftFront], wheelHitData[WheelID.RightFront]);
            vehicle.rollbarFront.Start();
            vehicle.rollbarRear = new(carRigidbody, wheelHitData[WheelID.LeftRear], wheelHitData[WheelID.RightRear]);
            vehicle.rollbarRear.Start();

            vehicle.differential = new(differentialConfig, 2);
            vehicle.differential.Start();

            vehicle.transmission = new(transmissionConfig);
            vehicle.transmission.Start();

            vehicle.clutch = new ClutchComponent(ClutchConfig);
            vehicle.clutch.Start();

            vehicle.engine = new EngineComponent(EngineConfig);
            vehicle.engine.Start();

            SetupTweaks();
        }


        private void OnDestroy()
        {
            vehicle.transmission.Shutdown();
        }
        #region Tweaking Car
        public void SetupTweaks()
        {
            springStrength = vehicle.suspension[WheelID.LeftFront].springStrength;
            damperStrength = vehicle.suspension[WheelID.LeftFront].springStrength;
            restLength = vehicle.suspension[WheelID.LeftFront].restLength;
            rollbarStrength = vehicle.rollbarFront.rollbarStrength;
        }
        public void OnValidate()
        {
            if (vehicle != null)
            {
                vehicle.suspension[WheelID.LeftFront ].springStrength = springStrength;
                vehicle.suspension[WheelID.RightFront].springStrength = springStrength;
                vehicle.suspension[WheelID.LeftRear  ].springStrength = springStrength;
                vehicle.suspension[WheelID.RightRear ].springStrength = springStrength;

                vehicle.suspension[WheelID.LeftFront ].damperStrength = damperStrength;
                vehicle.suspension[WheelID.RightFront].damperStrength = damperStrength;
                vehicle.suspension[WheelID.LeftRear  ].damperStrength = damperStrength;
                vehicle.suspension[WheelID.RightRear ].damperStrength = damperStrength;

                vehicle.suspension[WheelID.LeftFront ].restLength = restLength;
                vehicle.suspension[WheelID.RightFront].restLength = restLength;
                vehicle.suspension[WheelID.LeftRear  ].restLength = restLength;
                vehicle.suspension[WheelID.RightRear ].restLength = restLength;

                vehicle.rollbarFront.rollbarStrength = rollbarStrength;
                vehicle.rollbarRear.rollbarStrength = rollbarStrength;

            }
        }
        #endregion Tweaking Car

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

        public static Vehicle Setup()
        {
            return new Vehicle();
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