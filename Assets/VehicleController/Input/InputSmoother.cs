
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static vc.VehicleController;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

namespace vc
{
    public class InputSmoother:MonoBehaviour
    {
        [SerializeField] FloatVariable steerVariable;
        [SerializeField] FloatVariable throttleVariable;
        [SerializeField] FloatVariable brakeVariable;
        [SerializeField] FloatVariable handbrakeVariable;
                
        [Header("Input Configuration")]
        [SerializeField]
        float steerStrength;
        [SerializeField]
        float throttleStrength;

        [SerializeField]
        float brakeStrength;
        VehicleControllerInputActions inputActions;

        [SerializeField] float steerTrarget;
        [SerializeField] float throttleTarget;
        [SerializeField] float brakeTarget;
        [SerializeField] float handBrakeTarget;


        [SerializeField] float overrideValue;
        [SerializeField] bool OverrideThrottle;
        [SerializeField] bool OverrideBrake;

        private void Awake()
        {
            inputActions = new VehicleControllerInputActions();
            inputActions.Enable();
        }
        private void Update()
        {
            if (OverrideThrottle)
            {
                throttleTarget = overrideValue;
            }
            if (OverrideBrake)
            {
                brakeTarget= overrideValue;
            }

            updateValue(steerVariable, steerTrarget,steerStrength);
            updateValue(throttleVariable, throttleTarget, throttleStrength);
            updateValue(brakeVariable, brakeTarget, brakeStrength);
            updateValue(handbrakeVariable, handBrakeTarget, 100f);
        }

        private void updateValue(FloatVariable currentValue, float target, float strength)
        {
            if (currentValue.Value == target)
                return;

            var s = (target == 0f) ? strength * 2f : strength;
            currentValue.Value = Mathf.MoveTowards(currentValue.Value, target, Time.deltaTime * s);
        }

        private void OnEnable()
        {
            inputActions.VehicleControllerInputs.Steer.performed += Input_Steer;
            inputActions.VehicleControllerInputs.Steer.canceled += Input_Steer;

            inputActions.VehicleControllerInputs.Throttle.performed += Input_Throttle;
            inputActions.VehicleControllerInputs.Throttle.canceled += Input_Throttle;

            inputActions.VehicleControllerInputs.Brake.performed += Input_Brake;
            inputActions.VehicleControllerInputs.Brake.canceled += Input_Brake;

            inputActions.VehicleControllerInputs.HandBrake.performed += Input_Handbrake;
            inputActions.VehicleControllerInputs.HandBrake.canceled += Input_Handbrake;
        }

        private void OnDisable()
        {
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
                throttleTarget = ctx.ReadValue<float>();
        }
        public void Input_Brake(InputAction.CallbackContext ctx)
        {
                brakeTarget = ctx.ReadValue<float>();
        }

        public void Input_Steer(InputAction.CallbackContext ctx)
        {
                steerTrarget = ctx.ReadValue<float>();
        }
        public void Input_Handbrake(InputAction.CallbackContext ctx)
        {
                handBrakeTarget = ctx.ReadValue<float>();   
        }        
    }
}
