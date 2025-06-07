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


        Rigidbody rb;
        
        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
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
            
            vehicle.suspension[WheelID.LeftFront].Update(dt);
            vehicle.suspension[WheelID.RightFront].Update(dt);
            vehicle.suspension[WheelID.LeftRear].Update(dt);
            vehicle.suspension[WheelID.RightRear].Update(dt);

            float driveTorque = throttleInput.Value - brakeInput.Value ;

            vehicle.wheels[WheelID.LeftFront].Update(dt, driveTorque);
            vehicle.wheels[WheelID.RightFront].Update(dt, driveTorque);

            vehicle.wheels[WheelID.LeftRear].Update(dt, driveTorque);
            vehicle.wheels[WheelID.RightRear].Update(dt, driveTorque);

            vehicle.body.Update(dt);
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
        }
        Vehicle vehicle;

        private void SetupVehicle()
        {
            vehicle = new();


            // wheels 
            vehicle.wheels = new();
            foreach (var wc in wheelConfig) 
            {                
                var w = new WheelComponent(wc.Config, wc.ID);
                vehicle.wheels.Add(w.id, w);
            }

            // Setup Suspensions            
            vehicle.suspension = new();
            foreach (var susp in suspensionConfig)
            {
                vehicle.suspension.Add(susp.ID, new SuspensionComponent(susp.Config, vehicle.wheels[susp.ID], susp.mountPoint, rb));
            }

            vehicle.differential = new DifferentialComponent(differentialConfig);
            vehicle.transmission = new TransmissionComponent(transmissionConfig);
            vehicle.clutch = new ClutchComponent(ClutchConfig);
            vehicle.engine = new EngineComponent(EngineConfig);
            vehicle.body = new BodyComponent(bodyConfig, vehicle.suspension[WheelID.LeftFront].mountPoint, vehicle.suspension[WheelID.RightFront].mountPoint);
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