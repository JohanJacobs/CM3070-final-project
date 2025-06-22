using System;
using System.Collections.Generic;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class TransmissionComponent : IVehicleComponent, IDebugInformation
        {
            public float numberOfGears => gearRatios.Count - 1;
            public float GearRatio => ratio;
            public float CurrentGear => currentGear;
            TransmissionSO config;

            float currentGear = default;            
            FloatVariable gearUpInput;
            FloatVariable gearDownInput;

            bool isInGear = true;
            float ratio = default;
            float changeDuration = 0.2f;
            float gearShiftTime = 0.1f;
            List<float> gearRatios = new List<float> { 0, 3.675f, 2.375f, 1.761f, 1.346f, 1.062f, 0.842f };
            float reverseGearRatio = -3.545f;

            #region Transmission Component
            public TransmissionComponent(TransmissionSO config)
            {
                this.config = config;
                this.gearUpInput = config.gearUpInputVariable;
                this.gearDownInput = config.gearDownInputVariable;
            }

            private void ShiftUp(float v)
            {
                if (v < 1f)
                    return;
                
                // Not in gear and at highest gear
                if ((currentGear < numberOfGears) && isInGear)
                {
                    isInGear = false;
                    ratio = 0f;
                    ExecuteAfterDelay(gearShiftTime, () =>
                    {
                        currentGear += 1f; // TODO: MAke INT variable
                        UpdateRatio();
                        isInGear = true;
                    });
                }
            }

            private void ShiftDown(float v)
            {
                if (v < 1f)
                    return;
                                
                // check if we can down shift
                if ((currentGear > -1) && isInGear)
                {
                    isInGear = false;
                    ratio = 0f;
                    ExecuteAfterDelay(gearShiftTime, () =>
                    {
                        currentGear -= 1f; // TODO: MAke INT variable
                        UpdateRatio();
                        isInGear = true;                        
                    });
                }
            }

            void ExecuteAfterDelay(float seconds, Action action)
            {
                CoroutineHelper.ExecuteAfterDelay(seconds, action);
            }

            bool inReverseGear => (currentGear == -1f);
            void UpdateRatio()
            {
                ratio = (inReverseGear) ? reverseGearRatio : gearRatios[(int)currentGear];                
            }

            float diffiretialTorque = default;
            public float CaclulateDifferentialTorque(float inputTorque)
            {
                diffiretialTorque = inputTorque * ratio;
                return inputTorque * ratio;
            }

            float clutchVelocity = default;
            public float CalculateClutchVelocity(float inputVelocity)
            {
                clutchVelocity = inputVelocity * ratio;
                return inputVelocity * ratio;
            }

            string GetGearLabel()
            {
                switch (currentGear)
                {
                    case -1:
                        return "R";
                    case 0:
                        return "N";
                }
                return currentGear.ToString();
            }

            #endregion Transmission

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Differential;
            public void Shutdown()
            {
                gearUpInput.OnValueChanged -= ShiftUp;
                gearDownInput.OnValueChanged -= ShiftDown;
            }

            public void Start()
            {
                gearUpInput.OnValueChanged += ShiftUp;
                gearDownInput.OnValueChanged += ShiftDown;
            }

            public void Update(float dt)
            {
                
            }
            #endregion IVehicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {

            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"TRANSMISSION");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Gear: ({GetGearLabel()})");                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Raio: {GearRatio.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Diff.Trq: {diffiretialTorque.ToString("F2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Clutch.Vel: {clutchVelocity.ToString("F2")}");
                return yOffset;
            }
            #endregion IDebugInformation
        }

    }
}