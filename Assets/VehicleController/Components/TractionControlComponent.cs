using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;
using System.Collections.Generic;
using UnityEngine;

namespace vc
{
    namespace VehicleComponent
    {
        public interface ITractionControl
        {
            public void IncreaseStrength();
            public void DecreaseStrength();

            public float TractionControlThrottleAdjustFactor { get; }
        }

        public class TractionControlEngineComponent:IVehicleComponent<TractionControlEngineComponentStepParams>, IDebugInformation, ITractionControl
        {
            public float TractionControlThrottleAdjustFactor => tcInputFactor;

            float tcInputFactor = 1f;

            FloatVariable TCSetting;
            BoolVariable TCActive;
            BoolVariable TCEnabled;

            float TCMax;
            float TCMin;
            float TCStep = 1f;

            TractionControlSO config;
            Dictionary<WheelID, WheelComponent> wheels;
            FloatVariable throttleInput;
            StringVariable transmissionCurrentGear;

            #region TractionControlEngineComponent
            public TractionControlEngineComponent(TractionControlSO config, VehicleVariablesSO variables, Dictionary<WheelID,WheelComponent> wheels)
            {

                this.config = config;
                this.wheels = wheels;

                this.TCSetting = variables.TractionControlSetting;
                this.TCEnabled = variables.TractionControlEnabled;
                this.TCActive = variables.TractionControlActive;
                this.throttleInput = variables.throttle;
                this.transmissionCurrentGear = variables.currentGearText;
            }

            float driveWheelCount = 0f;
            float cumulativeSlip = 0f;
            float averageSlip = 0f;
            float CalculateAverageSlip()
            {
                cumulativeSlip = 0f;
                driveWheelCount = 0f;
                foreach (IDriveWheel w in wheels.Values)
                {
                    if (w.isDriveWheel)
                    {
                        cumulativeSlip += w.LongitudinalSlipRatio;
                        driveWheelCount += 1f;
                    }
                }

                return MathHelper.SafeDivide(cumulativeSlip,driveWheelCount);
            }
            #endregion TractionControlEngineComponent

            #region ITractionControl
            public void IncreaseStrength()
            {
                TCSetting.Value = Mathf.Clamp(TCSetting.Value + TCStep, TCMin, TCMax);
            }

            public void DecreaseStrength()
            {
                TCSetting.Value = Mathf.Clamp(TCSetting.Value - TCStep, TCMin, TCMax);
            }
            #endregion ITractionControl

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.TractionControl;

            public void Start()
            {
                this.TCEnabled.Value = this.config.TCEnabled;
                this.TCSetting.Value = this.config.TCDefault;
                this.TCMax = this.config.TCMax;
                this.TCMin = this.config.TCMin;
                this.TCStep = this.config.TCStep;
            }

            public void Shutdown()
            {

            }

            public void Step(TractionControlEngineComponentStepParams parameters)
            {
                // if TC is inactive break out 
                if (!TCEnabled.Value)
                    return;

                TCActive.Value = false;
                tcInputFactor = 1f;

                if (transmissionCurrentGear.Value == "N" || throttleInput.Value <= float.Epsilon)
                    return;

                // Update the traction control
                averageSlip = CalculateAverageSlip();         
                
                // Caclualte the new Factor to limit the engine by
                tcInputFactor = Mathf.Clamp(MathHelper.SafeDivide(TCStrength, Mathf.Abs(averageSlip)),0f,1f);

                // Set the flag
                TCActive.Value = tcInputFactor < 1f;
            }

            float TCStrength => TCMax - TCSetting.Value + TCMin;
            #endregion IVehicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {

            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"-TC Engine");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  TC Enabled: {TCEnabled.Value.ToString()}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  TC Active: {TCActive.Value.ToString()}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  TC Factor: {TractionControlThrottleAdjustFactor.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  TC Setting: {TCSetting.Value.ToString("F1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  TC Strength: {TCStrength.ToString("F1")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Avg Slip : {averageSlip.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  driveWheels: {driveWheelCount.ToString("F0")}");

                return yOffset;
            }

            #endregion IDEbugInformation
        }


        public class TractionControlEngineComponentStepParams
        {
            
        }

    }
}
