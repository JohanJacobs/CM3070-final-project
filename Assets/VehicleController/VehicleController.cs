using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;
using static vc.VehicleController;
/*
    visuals for skidding
    visuals for smoke // spinning
    FIX car freewheeling backwards having no revs
    fix for custom ForEach
 */
namespace vc
{
    public class VehicleController : MonoBehaviour
    {

        [Title("Configuration")]
        [SerializeField] VehicleConfiguration.WheelConfiguration[] WheelConfig;

        [Space,SerializeField] DifferentialSO differentialConfig;
        [SerializeField] TransmissionSO transmissionConfig;
        [SerializeField] EngineSO engineConfig;
        [SerializeField] ClutchSO clutchConfig;
        [SerializeField] BodySO bodyConfig;
        [SerializeField] BrakeSO brakeConfig;
        [SerializeField] AeroSO aeroConfig;
        [SerializeField] AntiRollbarSO frontAntiRollbar;
        [SerializeField] AntiRollbarSO rearAntiRollbar;

        Rigidbody carRigidbody;
                
        Vehicle vehicle;
        public Vehicle GetVehicle => vehicle;
        [SerializeField] VehicleVariablesSO vehicleVariables;

        public void Awake()
        {
            carRigidbody = GetComponent<Rigidbody>();
            if (carRigidbody == null)
            {
                Debug.LogError("Missing rigid body!");
            }
        }

        public void Start()
        {
            vehicle = Vehicle.Setup(
                carRigidbody,
                WheelConfig,
                bodyConfig,
                differentialConfig,
                transmissionConfig,
                clutchConfig,
                engineConfig,
                brakeConfig,
                aeroConfig,
                frontAntiRollbar,
                rearAntiRollbar,
                vehicleVariables);
        }

        public void Update()
        {
            vehicle.wheels.ForEach(w => {
                w.Value.UpdateVisuals(Time.deltaTime); 
            });
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            // Calculate the velocity of the vehicle
            Vector3 veloLS = carRigidbody.transform.InverseTransformDirection(carRigidbody.velocity);   
            
            // Start the physics step
            vehicle.body.Step(new (dt,veloLS));
            vehicle.aero.Step(new (veloLS));

            vehicle.suspension[WheelID.LeftFront].Step(new (dt));
            vehicle.suspension[WheelID.RightFront].Step(new (dt));
            vehicle.suspension[WheelID.LeftRear].Step(new (dt));
            vehicle.suspension[WheelID.RightRear].Step(new (dt));

            vehicle.rollbarFront.Step(new (dt));
            vehicle.rollbarRear.Step(new (dt));

            // DRIVE PHASE 
            var diffInpuTorque = vehicle.transmission.CaclulateDifferentialTorque(vehicle.clutch.clutchTorque);
            var driveTorque = vehicle.differential.CalculateWheelOutputTorque(
                    diffInpuTorque, 
                    vehicle.wheels[WheelID.LeftRear], 
                    vehicle.wheels[WheelID.RightRear],
                    dt);

            var frontBrakeTorque = vehicle.brake.frontBrakeTorque;
            var rearBrakeTorque = vehicle.brake.rearBrakeTorque;
            
            vehicle.wheels[WheelID.LeftFront ].Step(new (dt, 0f, frontBrakeTorque));
            vehicle.wheels[WheelID.RightFront].Step(new (dt, 0f,frontBrakeTorque));
            vehicle.wheels[WheelID.LeftRear].Step(new (dt, driveTorque[0], rearBrakeTorque));
            vehicle.wheels[WheelID.RightRear].Step(new (dt, driveTorque[1], rearBrakeTorque));

            // FEEDBACK PHASE
            var transVelo = vehicle.differential.CalculateTransmissionVelocity(
                    vehicle.wheels[WheelID.LeftRear].wheelAngularVelocity, 
                    vehicle.wheels[WheelID.RightRear].wheelAngularVelocity);
            var clutchVelo = vehicle.transmission.CalculateClutchVelocity(transVelo);

            vehicle.clutch.Update(clutchVelo, vehicle.transmission.GearRatio, vehicle.engine.engineAngularVelocity);
            vehicle.engine.Step(new (dt, vehicle.clutch.clutchTorque));
        }

        #region Vehicle
                
        private void OnDestroy()
        {
            Vehicle.Shutdown(vehicle);
            //vehicle.body.Shutdown();
            //vehicle.aero.Shutdown();
            //vehicle.engine.Shutdown();
            //vehicle.clutch.Shutdown();
            //vehicle.transmission.Shutdown();
            //vehicle.differential.Shutdown();
            //vehicle.rollbarFront.Shutdown();
            //vehicle.rollbarRear.Shutdown();
            //vehicle.brake.Shutdown();
            //vehicle.wheels.ForEach(w => w.Value.Shutdown());
            //vehicle.suspension.ForEach(s => s.Value.Shutdown());
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
            
            //yOffset = vehicle.body.OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.aero.OnGUI(xPos, yOffset, yStep);


            //yOffset = vehicle.engine.OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.clutch.OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.transmission.OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.differential.OnGUI(xPos, yOffset, yStep);

            //yOffset = vehicle.suspension[WheelID.LeftFront ].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.RightFront].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.LeftRear  ].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.suspension[WheelID.RightRear ].OnGUI(xPos, yOffset, yStep);

            yOffset = vehicle.wheels[WheelID.LeftFront ].OnGUI(xPos, yOffset, yStep);
            yOffset = vehicle.wheels[WheelID.RightFront].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.wheels[WheelID.LeftRear  ].OnGUI(xPos, yOffset, yStep);
            //yOffset = vehicle.wheels[WheelID.RightRear ].OnGUI(xPos, yOffset, yStep);

        }
        #endregion
    }
       

    namespace VehicleConfiguration
    {
        [System.Serializable]
        public class WheelConfiguration 
        {            
            public WheelID id;
            [HorizontalGroup("ScriptableObjects")]
            public WheelSO WheelConfig;
            [HorizontalGroup("ScriptableObjects")]
            public SuspensionSO SuspensionConfig;
            [HorizontalGroup("Setup")]
            public Transform wheelMesh;
            [HorizontalGroup("Setup")]
            public Transform suspMount;
        }


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