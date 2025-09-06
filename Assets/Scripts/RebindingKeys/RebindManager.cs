using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.HDROutputUtils;

public class RebindManager : MonoBehaviour
{
    VehicleControllerInputActions inputActions;

    private void Start()
    {
        inputActions = new VehicleControllerInputActions();        
    }

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;


    public void Rebind()
    {
        // test rebinding
        InputActionReference r = InputActionReference.Create(inputActions.VehicleControllerInputs.Throttle);
        rebindOperation = r.action.PerformInteractiveRebinding()
                    .WithControlsExcluding("Mouse")
                    .OnMatchWaitForAnother(0.01f)
                    .OnComplete(operation => RebindingComplete("Throttle", inputActions.VehicleControllerInputs.Throttle))
                    .Start();    
    }
    private void RebindingComplete(string label, InputAction action)
    {
        // get they binding index for the keyboard
        int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);

        var newKey = InputControlPath.ToHumanReadableString(action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        Debug.Log($"Completed {label} rebinding to {newKey}.");
        rebindOperation.Dispose();
    }
}
