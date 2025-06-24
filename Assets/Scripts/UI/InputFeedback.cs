
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFeedback : MonoBehaviour
{
    [SerializeField] FloatVariable SteerInput;
    [SerializeField] FloatVariable ThrottleInput;
    [SerializeField] FloatVariable  BrakeInput;
    [SerializeField] FloatVariable HandBrakeInput;

    [Header("UI Elements")]
    [SerializeField]
    TextMeshProUGUI steerText;
    [SerializeField]
    TextMeshProUGUI throttleText;
    [SerializeField]
    TextMeshProUGUI brakeText;
    [SerializeField]
    TextMeshProUGUI handbrakeText;
    private void OnDisable()
    {
        SteerInput.OnValueChanged     -= UpdateDisplay;
        ThrottleInput.OnValueChanged  -= UpdateDisplay;
        BrakeInput.OnValueChanged     -= UpdateDisplay;
        HandBrakeInput.OnValueChanged -= UpdateDisplay; 
    }
    private void OnEnable()
    {
        SteerInput.OnValueChanged     += UpdateDisplay;
        ThrottleInput.OnValueChanged  += UpdateDisplay;
        BrakeInput.OnValueChanged     += UpdateDisplay;
        HandBrakeInput.OnValueChanged += UpdateDisplay;
    }

    void UpdateDisplay(float value)
    {
        steerText.text = $"Steer : {SteerInput.Value.ToString("F2")}";
        throttleText.text = $"Throttle : {ThrottleInput.Value.ToString("F2")}";
        brakeText.text = $"Brake : {BrakeInput.Value.ToString("F2")}";
        handbrakeText.text = $"Hand Brake : {HandBrakeInput.Value.ToString("f0")}";
    }
}
