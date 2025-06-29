using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using TMPro;
using UnityEngine;
using vc;
using vc.VehicleComponent;

public class UIDisplayWheelStats : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] VehicleController vc;
    [SerializeField] WheelID wheelId;
    WheelComponent wc;

    [Header("UI Objects")]
    [SerializeField] TextMeshProUGUI slipAngleUI;
    [SerializeField] TextMeshProUGUI slipAngleDynamicUI;
    [SerializeField] TextMeshProUGUI FnUI;
    [SerializeField] TextMeshProUGUI FzUI;
    [SerializeField] TextMeshProUGUI FxUI;
    public void Start()
    {
        wc = vc.GetVehicle.wheels[wheelId];
    }

    public void LateUpdate()
    {
        slipAngleUI.text = $"SA {wc.LateralSlipAngle.ToString("F0")}";
        slipAngleDynamicUI.text = $"dSA {wc.SlipAngleDynamic.ToString("F0")}";
        FnUI.text = $"Fn {wc.Fn.ToString("F0")}";
        FzUI.text = $"Fz {wc.Fz.ToString("F0")}";
        FxUI.text = $"Fx {wc.Fx.ToString("F0")}";
    }
}
