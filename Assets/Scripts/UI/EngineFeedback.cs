using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EngineFeedback : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField]
    FloatVariable engineTorque;
    [SerializeField]
    FloatVariable engineRPM;
    [SerializeField]
    FloatVariable clutch;
    [SerializeField]
    FloatVariable transmission;
    [SerializeField]
    FloatVariable differential;

    [Header("UI")]
    [SerializeField]
    TextMeshProUGUI rpmText;
    [SerializeField]
    TextMeshProUGUI engineTorqueText;
    [SerializeField]
    TextMeshProUGUI clutchText;
    [SerializeField]
    TextMeshProUGUI TransmissionText;
    [SerializeField]
    TextMeshProUGUI DifferentialText;


    private void OnEnable()
    {
        engineTorque.OnValueChanged += EngineTorque_OnValueChanged;
        engineRPM.OnValueChanged += engineRPM_OnValueChanged;
    }
    private void OnDisable()
    {
        engineTorque.OnValueChanged -= EngineTorque_OnValueChanged;
        engineRPM.OnValueChanged -= engineRPM_OnValueChanged;
    }

    private void EngineTorque_OnValueChanged(float value)
    {
        engineTorqueText.text = $"Torque: {value.ToString("F1")}";
        
    }

    private void engineRPM_OnValueChanged(float value)
    {
        engineTorqueText.text = $"RPM: {value.ToString("F1")}";
    }

}
