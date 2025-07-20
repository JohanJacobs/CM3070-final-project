using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using vc;
using vc.VehicleComponent;

public class UITransmissionTypeChanger : MonoBehaviour
{
    [SerializeField] TransmissionTypeVariable variable;
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] TMPro.TMP_Dropdown dropdown;



    private void Start()
    {
        label.text = variable.VariableLabel;
        dropdown.options.Clear();
        foreach( var o in Enum.GetValues(typeof(TransmissionComponent.TransmissionType)))
        {            
            dropdown.options.Add(new TMP_Dropdown.OptionData(o.ToString()));
        }
    }
    private void OnEnable()
    {
        dropdown.onValueChanged.AddListener(Dropdown_OnValueChanged);
    }

    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(Dropdown_OnValueChanged);
    }


    void Dropdown_OnValueChanged(int option)
    {
        string s = dropdown.options[option].text;
        variable.Value = (TransmissionComponent.TransmissionType)Enum.Parse(typeof(TransmissionComponent.TransmissionType),s);
    }
}
