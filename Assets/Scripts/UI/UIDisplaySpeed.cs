using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UIDisplaySpeed : MonoBehaviour
{
    [SerializeField] FloatVariable variable;
    [SerializeField] string preFix;
    [SerializeField] string postFix;
    [SerializeField] TextMeshProUGUI textDisplay;

    public void Start()
    {
        Variable_OnValueChanged(variable.Value);
    }

    private void OnEnable()
    {
        variable.OnValueChanged += Variable_OnValueChanged;        
    }

    private void OnDisable()
    {
        variable.OnValueChanged -= Variable_OnValueChanged;
    }

    void Variable_OnValueChanged(float newValue)
    {
        textDisplay.text = preFix + (Mathf.RoundToInt(newValue)).ToString("f0") + postFix;
    }
}
