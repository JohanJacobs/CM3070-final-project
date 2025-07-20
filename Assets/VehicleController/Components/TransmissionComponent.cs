using System;
using System.Collections.Generic;
using UnityEngine;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
    namespace VehicleComponent
    {
        public class TransmissionComponent : IVehicleComponent<TransmissionStepParameters>, IDebugInformation
        {
            public enum TransmissionType
            {
                Automatic,
                Manual
            }

            public float numberOfGears => gearCount.Value;
            FloatVariable gearCount;
            public float GearRatio => ratio;
            public float CurrentGear => currentGear;
            TransmissionSO config;

            float currentGear = default;            
            FloatVariable gearUpInput;
            FloatVariable gearDownInput;

            StringVariable currentGearText;
            FloatVariable gearShiftTime;
            FloatVariable reverseGearRatio;

            FloatVariable efficiency;

            FloatVariable throttleInput;
            FloatVariable brakeInput;
            bool isInGear = true;
            float ratio = default;
            TransmissionType transmissionType;

            //List<float> gearRatios = new List<float> 0, 3.658f, 1.93f, 1.28f, 0.95f, 0.76f, 0.76f };// fiesta
            List<FloatVariable> gearRatios;
            FloatVariable gearRatioNeutral;

            #region Transmission Component
            public TransmissionComponent(TransmissionSO config, VehicleVariablesSO variables)
            {
                this.config = config;

                // input
                this.gearUpInput = variables.gearUp;
                this.gearDownInput = variables.gearDown;

                // setup variables
                this.currentGearText = variables.currentGearText;
                this.gearShiftTime = variables.gearShiftTime;
                this.reverseGearRatio = variables.gearRatioReverse;
                this.gearCount = variables.gearCount;
                this.throttleInput = variables.throttle;
                this.brakeInput = variables.brake;

                this.gearRatioNeutral = variables.gearRatioNeutral;
                gearRatios = new();
                gearRatios.Add(gearRatioNeutral);
                gearRatios.Add(variables.gearRatioFirst);
                gearRatios.Add(variables.gearRatioSecond);
                gearRatios.Add(variables.gearRatioThird);
                gearRatios.Add(variables.gearRatioForth);
                gearRatios.Add(variables.gearRatioFifth);
                gearRatios.Add(variables.gearRatioSixth);

                this.efficiency = variables.transmissionEfficiency;
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
                    ExecuteAfterDelay(gearShiftTime.Value, () =>
                    {
                        currentGear += 1f; // TODO: MAke INT variable
                        UpdateRatio();
                        isInGear = true;
                        SetCurrentGearText(currentGear);
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
                    ExecuteAfterDelay(gearShiftTime.Value, () =>
                    {
                        currentGear -= 1f; // TODO: MAke INT variable
                        UpdateRatio();
                        isInGear = true; 
                        SetCurrentGearText(currentGear);
                    });
                }
            }

            void SetCurrentGearText(float gear)
            {
                if (gear == -1f)
                {
                    currentGearText.Value = "R";
                    return;
                }

                if (gear == 0f)
                {
                    currentGearText.Value = "N";
                    return;
                }

                currentGearText.Value = currentGear.ToString("F0");
            }

            void ExecuteAfterDelay(float seconds, Action action)
            {
                CoroutineHelper.ExecuteAfterDelay(seconds, action);
            }

            bool inReverseGear => (currentGear == -1f);
            void UpdateRatio()
            {
                ratio = (inReverseGear) ? reverseGearRatio.Value : gearRatios[(int)currentGear].Value;
            }

            float diffiretialTorque = default;

            public float CaclulateDifferentialTorque(float inputTorque)
            {
                diffiretialTorque = inputTorque * ratio * efficiency.Value;
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
            public ComponentTypes GetComponentType() => ComponentTypes.Transmission;
            public void Shutdown()
            {
                gearUpInput.OnValueChanged -= ShiftUp;
                gearDownInput.OnValueChanged -= ShiftDown;
            }

            public void Start()
            {
                this.gearRatioNeutral.Value = 0f;

                gearCount.Value = this.config.gearRatios.Length;

                this.reverseGearRatio.Value = this.config.reverseGearRatio;
                this.efficiency.Value = this.config.efficiency;
                this.gearShiftTime.Value = this.config.gearShiftTime;

                for (int i = 0; i < numberOfGears; i++) 
                {
                    gearRatios[i+1].Value = this.config.gearRatios[i];
                }

                currentGear = 0f;
                SetCurrentGearText(currentGear);
                gearUpInput.OnValueChanged += ShiftUp;
                gearDownInput.OnValueChanged += ShiftDown;

                transmissionType = this.config.TransmissionType;

                autoShiftTimeTarget = this.config.gearShiftTime * 3f;
            }

            public void Step(TransmissionStepParameters parameters)
            {
                if (transmissionType == TransmissionType.Automatic)
                {
                    var wheelCircumference = parameters.wheelRadiusMeter * 2f * Mathf.PI;
                    CheckShift(parameters.engine, parameters.differential, parameters.vehicle, wheelCircumference);
                    lastAutoShiftTime += parameters.deltaTime;
                }
            }

            float gearDownSpdTarget;
            float rpmShiftDownTarget;

            float gearUpSpdTarget;

            float lastAutoShiftTime = 0f;
            float autoShiftTimeTarget;
           
            void CheckShift(IEngineRPM engine,IRatio differential,ISpeed vehicle, float wheelCircumference)
            {
                if (lastAutoShiftTime < autoShiftTimeTarget) return;

                bool inFirstGear = currentGear == 1f;
                bool inReverse = currentGear == -1;
                bool inNeutral = currentGear == 0f;
                bool inDriveGear = currentGear > 0f;
                
                bool isIdling = engine.CurrentRPM == engine.IdleRRM;

                bool throttleDown = throttleInput.Value > 0f;
                bool brakeDown = brakeInput.Value > 0f;


                // TODO : should be a state machine 
                // Neutral to "Reverse" of "First"
                float rpmTarget = engine.IdleRRM * 1.2f;
                if (inNeutral && throttleDown && !isIdling)
                {
                    // shift up
                    ShiftUp(1f);
                    lastAutoShiftTime = 0f;
                    return;
                }

                // Go to Reverse Gear 
                if (inNeutral && brakeDown && isIdling)
                {
                    ShiftDown(1f);
                    lastAutoShiftTime = 0f;
                    return;
                }

                // go to neutral from reverse
                if (inReverse && isIdling)
                {
                    ShiftUp(1f);
                    lastAutoShiftTime = 0f;
                    return;
                }

                // GOTO neutral from 1st 
                if (inFirstGear && (!throttleDown || brakeDown) && isIdling)
                {
                    ShiftDown(1f);
                    lastAutoShiftTime = 0f;
                    return;
                }

                if (inDriveGear)
                {
                    // GEAR UP
                    var driveTrainRatio = differential.Ratio * ratio;
                    var rpmShiftUpTarget = engine.RedlineRPM * 0.75f;
                    var uppserSpdRPM = MathHelper.SafeDivide(rpmShiftUpTarget, driveTrainRatio);
                    gearUpSpdTarget = PhysicsHelper.Conversions.MPerMinuteToKMH(uppserSpdRPM * wheelCircumference);

                    if (vehicle.SpeedKMH > gearUpSpdTarget && throttleDown)
                    {
                        ShiftUp(1f);
                        lastAutoShiftTime = 0f;
                        return;
                    }

                    //GEAR DOWN
                    rpmShiftDownTarget = engine.RedlineRPM * 0.5f;
                    var lowerSpdRPM = MathHelper.SafeDivide(rpmShiftDownTarget, driveTrainRatio);
                    gearDownSpdTarget = (lowerSpdRPM * wheelCircumference) * 60f / 1000f;
                    gearDownSpdTarget = (currentGear == 1f) ? 0f : gearDownSpdTarget;
                    
                    if (vehicle.SpeedKMH  < gearDownSpdTarget && currentGear > 1f)
                    {                        
                        ShiftDown(1f);
                        lastAutoShiftTime = 0f;
                        return;
                    }
                }
            }


            #endregion IVehicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {

            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"TRANSMISSION");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Gear Up SPD: ({gearUpSpdTarget.ToString("F2")})");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Gear Dwn SPD: ({gearDownSpdTarget.ToString("F2")})");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Gear Dwn RPM: ({rpmShiftDownTarget.ToString("F2")})");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  BrakeInput: ({brakeInput.Value.ToString("F2")})");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  currentGear: ({currentGear.ToString("F2")})");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  lastShiftTime: ({lastAutoShiftTime.ToString("F2")})");

                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Gear: ({GetGearLabel()})");                
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Raio: {GearRatio.ToString("F3")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Diff.Trq: {diffiretialTorque.ToString("F2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Clutch.Vel: {clutchVelocity.ToString("F2")}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  in Gear: {isInGear.ToString()}");
                return yOffset;
            }
            #endregion IDebugInformation
        }
        #region Parameters
        public class TransmissionStepParameters
        {
            public TransmissionStepParameters(IRatio differentialComponent,IEngineRPM engineComponent, ISpeed vehicleBodyComponent,float dt, float wheelRadiusMeter)
            {
                this.differential = differentialComponent;
                this.engine = engineComponent;
                this.vehicle = vehicleBodyComponent;
                this.deltaTime = dt;
                this.wheelRadiusMeter = wheelRadiusMeter;
            }

            public float deltaTime;
            public IRatio differential;
            public IEngineRPM engine;
            public ISpeed vehicle;
            public float wheelRadiusMeter;
        }
        #endregion Parameters
        
        

    }
}