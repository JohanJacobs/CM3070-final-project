using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using vc;
using vc.VehicleComponentsSO;

public class UIVariableDropDown : MonoBehaviour
{
    [SerializeField] ScriptableObjectVariable variable;
    [SerializeField] WheelSO[] options;

    [SerializeField] TMPro.TMP_Dropdown dropdown;
    
    private void Awake()
    {
        dropdown.options.Clear();
        foreach (var i in options)
        {
            var n = i.name.Replace("Wheel", "").Replace("SO", "");
            dropdown.options.Add(new TMP_Dropdown.OptionData(n));
        }


    }
    private void OnEnable()
    {
        dropdown.onValueChanged.AddListener(TMPDropdown_OnValueChanged);
        variable.OnValueChanged += ScriptableObjectVariable_OnValueChanged;
    }
    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(TMPDropdown_OnValueChanged);
    }
    void TMPDropdown_OnValueChanged(int option)
    {
        variable.Value = options[dropdown.value];
    }

    void ScriptableObjectVariable_OnValueChanged(ScriptableObjectBase b)
    {

    }
}
