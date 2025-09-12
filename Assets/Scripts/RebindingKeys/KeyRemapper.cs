
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using vc;

public class KeyRemapper : MonoBehaviour
{
    [SerializeField] InputSmoother.VehicleInputActionsEnum keyInputAction;
    [SerializeField] TextMeshProUGUI keyText;
    [SerializeField] TextMeshProUGUI labelText;
    [SerializeField] Button remapButton;
    [SerializeField] InputActionReference inputAction;    
    string waitingText = " ..Waiting.. ";

    private InputActionRebindingExtensions.RebindingOperation rebindOperation;

    
    private void Start()
    {
        labelText = transform.Find("InputLabel").GetComponent<TextMeshProUGUI>();
        keyText = transform.Find("KeyText").GetComponent<TextMeshProUGUI>();
        remapButton = transform.Find("RemapButton").GetComponent<Button>();
        remapButton.onClick.AddListener(() => RebindKey());
        GetInputAction();
    }
    private void OnEnable()
    {
        // set the current key map
        if (inputAction == null)
        {
            GetInputAction();
        }

        labelText.text = inputAction.action.name;

        UpdateKeyText();
        
    }

    private void UpdateKeyText()
    {
        if (inputAction.action.bindings[0].isComposite)
        {
            var totalBindings = inputAction.action.bindings.Count;
            var bindings = inputAction.action.bindings;
            var currentIDX = 1;

            var mappingText = "";
            while (currentIDX < totalBindings && bindings[currentIDX].isPartOfComposite)
            {
                //mappingText += bindings[currentIDX].effectivePath  + "/";
                mappingText += GetKeyText( bindings[currentIDX].effectivePath)+ "/";
                currentIDX++;
            }

            //Debug.Log(mappingText);
            keyText.text = mappingText.Remove(mappingText.Length-1); // delete the last /

        }
        else
        {
            keyText.text = GetKeyText(GetControlPath(0));
        }
    
    }

    private void GetInputAction()
    {
        var inputSMoother = FindAnyObjectByType<vc.InputSmoother>();
        inputAction = inputSMoother.GetInputActionReference(keyInputAction);
    }

    private string GetControlPath(int controlsBindingIndex = 0)
    {
        int bindingIndex = inputAction.action.GetBindingIndexForControl(inputAction.action.controls[controlsBindingIndex]);
        return inputAction.action.bindings[bindingIndex].effectivePath;
    }

    private string GetKeyText(string effectivePath)
    {
        var keyText = InputControlPath.ToHumanReadableString(effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        return keyText;
    }


    public void RebindKey()
    {
        EventSystem.current.SetSelectedGameObject(null); // deselect all objects                 
        keyText.text = waitingText;
        CreateRebindingAction();
    }

    string currentCompositeGroup = "";
    private void CreateRebindingAction(int index = 0)
    {
        inputAction.ToInputAction().Disable();
        if (inputAction.action.bindings[index].isComposite)
        {
            index = index + 1;
            currentCompositeGroup = inputAction.action.bindings[index].groups;
        }
        else
        {
            currentCompositeGroup = "";
        }
                    
        rebindOperation = inputAction.action.PerformInteractiveRebinding(index)
                .WithControlsExcluding("Mouse") // exclude mouse stuff
                .WithCancelingThrough("<Keyboard>/escape") // dont use the escape key as a valid bind but as a cancel event
                .OnMatchWaitForAnother(0.01f)
                .OnComplete(operation => RebindingComplete(index))
                .WithBindingGroup("Keyboard")
                .Start();
    }

    private void RebindingComplete(int bindingIDX)
    {        
        // get they binding index for the keyboard        
        rebindOperation.Dispose();
        UpdateKeyText();
        if (inputAction.action.bindings[bindingIDX].isPartOfComposite &&
            bindingIDX + 1 < inputAction.action.bindings.Count &&
            inputAction.action.bindings[bindingIDX + 1].groups == currentCompositeGroup)
        {
                    CreateRebindingAction(bindingIDX + 1);
        }
        else
        {
            inputAction.ToInputAction().Enable();
        }

        //if (inputAction.action.bindings[bindingIDX].isPartOfComposite)
        //{
        //    if (bindingIDX+1 < inputAction.action.bindings.Count)
        //    {
        //        if (inputAction.action.bindings[bindingIDX+1].groups == currentCompositeGroup)
        //            CreateRebindingAction(bindingIDX + 1);
        //        else
        //            inputAction.ToInputAction().Enable();
        //    }
        //    else
        //        inputAction.ToInputAction().Enable();
        //}
        //else
        //{
        //    inputAction.ToInputAction().Enable();
        //}
    }
}
