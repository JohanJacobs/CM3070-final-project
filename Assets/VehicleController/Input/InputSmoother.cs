
using UnityEngine;
using UnityEngine.InputSystem;

namespace vc
{
    public class InputSmoother:MonoBehaviour
    {
        [Header("Driving")]
        [SerializeField] FloatVariable steerVariable;
        [SerializeField] FloatVariable throttleVariable;
        [SerializeField] FloatVariable brakeVariable;
        [SerializeField] FloatVariable handbrakeVariable;

        [Header("Transmission")]
        [SerializeField] FloatVariable GearUpVariable;
        [SerializeField] FloatVariable GearDownVariable;

        [Header("Input Configuration")]
        [SerializeField, Range(0f,1f)] float SteerSensitivity;
        VehicleControllerInputActions inputActions;
        [SerializeField] float steerStrength;
        [SerializeField] float throttleStrength;
        [SerializeField] float brakeStrength;
        [SerializeField] float handbrakeStrength = 100f;
        [SerializeField] float gearUpStrength = 100f;
        [SerializeField] float gearDownStrength = 100f;

        [Header("DebugInfo")]
        [SerializeField] float steerTrarget;
        [SerializeField] float throttleTarget;
        [SerializeField] float brakeTarget;
        [SerializeField] float handBrakeTarget;
        [SerializeField] float gearUpTarget;
        [SerializeField] float gearDownTarget;


        [Header("Override")]
        [SerializeField] float overrideValue;
        [SerializeField] bool OverrideThrottle;
        [SerializeField] bool OverrideBrake;


        

        private void Start()
        {
            CreateInputActions();            
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
            updateValue(handbrakeVariable, handBrakeTarget, handbrakeStrength);
            updateValue(GearUpVariable, gearUpTarget, gearUpStrength);
            updateValue(GearDownVariable, gearDownTarget, gearDownStrength);
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
            CreateInputActions();
            inputActions.VehicleControllerInputs.Steer.performed += Input_Steer;
            inputActions.VehicleControllerInputs.Steer.canceled += Input_Steer;

            inputActions.VehicleControllerInputs.Throttle.performed += Input_Throttle;
            inputActions.VehicleControllerInputs.Throttle.canceled += Input_Throttle;

            inputActions.VehicleControllerInputs.Brake.performed += Input_Brake;
            inputActions.VehicleControllerInputs.Brake.canceled += Input_Brake;

            inputActions.VehicleControllerInputs.HandBrake.performed += Input_Handbrake;
            inputActions.VehicleControllerInputs.HandBrake.canceled += Input_Handbrake;

            inputActions.VehicleControllerInputs.GearUp.performed += Input_GearUp;
            inputActions.VehicleControllerInputs.GearUp.canceled += Input_GearUp;
            ;

            inputActions.VehicleControllerInputs.GearDown.performed += Input_GearDown;
            inputActions.VehicleControllerInputs.GearDown.canceled += Input_GearDown;

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

            inputActions.VehicleControllerInputs.GearUp.performed -= Input_GearUp;
            inputActions.VehicleControllerInputs.GearUp.canceled -= Input_GearUp;

            inputActions.VehicleControllerInputs.GearDown.performed -= Input_GearDown;
            inputActions.VehicleControllerInputs.GearDown.canceled -= Input_GearDown;
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

            steerTrarget = ctx.ReadValue<float>() * SteerSensitivity;
        }
        public void Input_Handbrake(InputAction.CallbackContext ctx)
        {
            handBrakeTarget = ctx.ReadValue<float>();   
        }        

        public void Input_GearUp(InputAction.CallbackContext ctx)
        {
            gearUpTarget = ctx.ReadValue<float>();
        }
        public void Input_GearDown(InputAction.CallbackContext ctx)
        {
            gearDownTarget = ctx.ReadValue<float>();
        }


        public enum VehicleInputActionsEnum
        {
            Throttle,            
            Brake,
            HandBrake,
            Steer,
            GearUp,
            GearDown,

        }
        public InputActionReference GetInputActionReference(VehicleInputActionsEnum actionEnum)
        {
            CreateInputActions();
            switch (actionEnum)
            {
                case VehicleInputActionsEnum.Throttle:
                    return InputActionReference.Create(inputActions.VehicleControllerInputs.Throttle);
                case VehicleInputActionsEnum.Brake:
                    return InputActionReference.Create(inputActions.VehicleControllerInputs.Brake);
                case VehicleInputActionsEnum.HandBrake:
                    return InputActionReference.Create(inputActions.VehicleControllerInputs.HandBrake);
                case VehicleInputActionsEnum.GearUp:
                    return InputActionReference.Create(inputActions.VehicleControllerInputs.GearUp);
                case VehicleInputActionsEnum.GearDown:
                    return InputActionReference.Create(inputActions.VehicleControllerInputs.GearDown);
                case VehicleInputActionsEnum.Steer:
                    return InputActionReference.Create(inputActions.VehicleControllerInputs.Steer);
                default:
                    return null;
            }
        }

        private void CreateInputActions()
        {
            if (inputActions == null)
            {
                inputActions = new VehicleControllerInputActions();
                inputActions.Enable();
            }
        }
    }
}
