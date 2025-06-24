using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDisplayStringVariable : MonoBehaviour
{
    [SerializeField] StringVariable variable;
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

    void Variable_OnValueChanged(string newvalue)
    {
        textDisplay.text = preFix + newvalue + postFix;
    }
}
