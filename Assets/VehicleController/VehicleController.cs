using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;

namespace vc
{
    public class VehicleController : MonoBehaviour
    {
        [Title("Configuration")]
        [SerializeField]
        VehicleConfiguration.WheelConfigData[] wheelConfig;
        [SerializeField,Space]
        VehicleConfiguration.SuspensionConfigData[] suspensionConfig;

        Dictionary<ComponentTypes, List<IVehicleComponent>> components;

        Rigidbody rb;
        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Missing rigid body!");
            }

            SetupVehicle();
            SetupInputHandlers();
        }

        private void OnEnable()
        {
            SubscribeToInput();
        }
        private void OnDisable()
        {
            UnsubscribeToInput();
        }
        public void Update()
        {            
        }
        private void FixedUpdate()
        {
            foreach(var susp in components[ComponentTypes.Suspension])
            {
                susp.Update(Time.fixedDeltaTime);
            }
        }
        #region Vehicle
        private void SetupVehicle()
        {
            components = new();

            // wheels 
            Dictionary<WheelID, WheelComponent> wheels = new();
            List<IVehicleComponent> wheelComponents = new();
            foreach (var wc in wheelConfig) 
            {                
                var w = new WheelComponent(wc.Config, wc.ID);
                wheels.Add(w.id, w);
                wheelComponents.Add(w);
            }

            components.Add(ComponentTypes.Wheel, wheelComponents);

            // Setup Suspensions
            List<IVehicleComponent> suspensionComponents = new();
            foreach (var susp in suspensionConfig)
            {                
                suspensionComponents.Add(new SuspensionComponent(susp.Config, wheels[susp.ID],susp.mountPoint,rb));
            }
            components.Add(ComponentTypes.Suspension, suspensionComponents);
        }
        #endregion Vehicle


        #region Input Management
        VehicleControllerInputActions inputActions;
        bool inputActionsSetupComplete =false;
        public enum InputSmootherObjectType
        {
            Steer,
            Brake,
            Throttle,
            Handbrake
        }
        Dictionary<InputSmootherObjectType,InputSmoother> inputSmoothers;
        [Header("Input Configuration")]
        [SerializeField]
        float steerStrength;
        [SerializeField]
        float throttleStrength;
        [SerializeField]
        float breakPedalStrength;

        private void SetupInputHandlers()
        {
            inputSmoothers = new();
            inputSmoothers.Add(InputSmootherObjectType.Steer, new InputSmoother(steerStrength, "Steer"));
            inputSmoothers.Add(InputSmootherObjectType.Throttle, new InputSmoother(throttleStrength, "Throttle"));
            inputSmoothers.Add(InputSmootherObjectType.Brake, new InputSmoother(breakPedalStrength, "Brake"));
            inputSmoothers.Add(InputSmootherObjectType.Handbrake, new InputSmoother(100f, "Handbrake"));
            inputActions = new VehicleControllerInputActions();
            inputActions.Enable();
            inputActionsSetupComplete = true;

        }

        private void SubscribeToInput()
        {
            if (!inputActionsSetupComplete)
            {
                Debug.LogError("Please call SetupInputHandlers. Input is currently disabled");
                return;
            }

            inputActions.VehicleControllerInputs.Steer.performed += Input_Steer;
            inputActions.VehicleControllerInputs.Steer.canceled += Input_Steer;

            inputActions.VehicleControllerInputs.Throttle.performed += Input_Throttle;
            inputActions.VehicleControllerInputs.Throttle.canceled += Input_Throttle;

            inputActions.VehicleControllerInputs.Brake.performed += Input_Brake;
            inputActions.VehicleControllerInputs.Brake.canceled += Input_Brake;

            inputActions.VehicleControllerInputs.HandBrake.performed += Input_Handbrake;
            inputActions.VehicleControllerInputs.HandBrake.canceled += Input_Handbrake;
        }

        private void UnsubscribeToInput()
        {
            if (!inputActionsSetupComplete)
            {
                Debug.LogError("Please call SetupInputHandlers. Input is currently disabled");
                return;
            }

            inputActions.VehicleControllerInputs.Steer.performed -= Input_Steer;
            inputActions.VehicleControllerInputs.Steer.canceled -= Input_Steer;

            inputActions.VehicleControllerInputs.Throttle.performed -= Input_Throttle;
            inputActions.VehicleControllerInputs.Throttle.canceled -= Input_Throttle;

            inputActions.VehicleControllerInputs.Brake.performed -= Input_Brake;
            inputActions.VehicleControllerInputs.Brake.canceled -= Input_Brake;

            inputActions.VehicleControllerInputs.HandBrake.performed -= Input_Handbrake;
            inputActions.VehicleControllerInputs.HandBrake.canceled -= Input_Handbrake;
        }
        public void Input_Throttle(InputAction.CallbackContext ctx)
        {
            Debug.Log("Throttle");
            Debug.Log(ctx);
            inputSmoothers[InputSmootherObjectType.Throttle].SetTarget(ctx.ReadValue<float>());
        }
        public void Input_Brake(InputAction.CallbackContext ctx)
        {
            Debug.Log("Brake");
            Debug.Log(ctx);
            inputSmoothers[InputSmootherObjectType.Brake].SetTarget(ctx.ReadValue<float>());
        }

        public void Input_Steer(InputAction.CallbackContext ctx)
        {
            Debug.Log("steer");
            Debug.Log(ctx);
            inputSmoothers[InputSmootherObjectType.Steer].SetTarget(ctx.ReadValue<float>());
        }
        public void Input_Handbrake(InputAction.CallbackContext ctx)
        {
            Debug.Log("handbrake");
            Debug.Log(ctx);
            inputSmoothers[InputSmootherObjectType.Handbrake].SetTarget(ctx.ReadValue<float>());
        }

        #endregion Input Management

        #region DebugInformation

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            foreach (IDebugInformation c in components[ComponentTypes.Suspension])
            {
                c.DrawGizmos();
            }
            foreach (IDebugInformation c in components[ComponentTypes.Wheel])
            {
                c.DrawGizmos();
            }
        }

        private void OnGUI()
        {
            float yOffset = 10f;
            float yStep = 20f;
            float xPos = 10f;
            foreach (IDebugInformation c in components[ComponentTypes.Suspension]) 
            {
                yOffset = c.OnGUI(xPos, yOffset, yStep);
            }
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