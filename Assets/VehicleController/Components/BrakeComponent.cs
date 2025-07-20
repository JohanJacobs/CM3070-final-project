using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
    namespace VehicleComponent
    {
       
        public class BrakeComponent : IVehicleComponent<BrakeComponenetStepParameters>,IDebugInformation
        {
            BrakeSO config;
            FloatVariable brakeInput;
            FloatVariable handbrakeInput;
            FloatVariable maxBrakeTorque;
            FloatVariable brakeBalance;
            BoolVariable ABSActive;
            BoolVariable ABSEnabled;
            Dictionary<WheelID, bool> wheelABSState = new();            

            #region Brake Component
            public BrakeComponent(BrakeSO brakeConfig, VehicleVariablesSO variables)
            {
                this.config = brakeConfig;

                // setup variables
                this.brakeInput = variables.brake;
                this.handbrakeInput = variables.handBrake;

                this.maxBrakeTorque = variables.brakeTorque;
                this.brakeBalance = variables.brakeBalance;
                this.ABSActive = variables.ABSActive;
                this.ABSEnabled = variables.ABSEnabled;

                // setup data structure                
                wheelABSState.Add(WheelID.LeftFront, false);
                wheelABSState.Add(WheelID.RightFront, false);
                wheelABSState.Add(WheelID.LeftRear, false);
                wheelABSState.Add(WheelID.RightRear, false);
            }

            float rearBrakeTorque => brakeInput.Value * maxBrakeTorque.Value * brakeBalance.Value; // nm
            float frontBrakeTorque => Mathf.Min(brakeInput.Value * maxBrakeTorque.Value * (1f - brakeBalance.Value) + handbrakeInput.Value * maxBrakeTorque.Value, maxBrakeTorque.Value); //nm
            
            float absTargetSlipRatio = 0.8f;
            float minABSTorqueRatio = 0.1f;// 10% minimum force;
            bool isABSEnabled => ABSEnabled.Value;
            public float CalculateBrakeTorque (WheelComponent wheel)
            {
                var brakeTorque = isFrontWheel(wheel.id) ? frontBrakeTorque : rearBrakeTorque;

                IABS abs = wheel;
                // If the ABS system is disabled just return the brake torque
                if (!isABSEnabled) return brakeTorque;
                // Calculate the new brake torque based on the ABS system
                return ABSAdjustedBrakeToqrue(abs, wheel.LongitudinalSlipRatio,brakeTorque);
            }

            bool isFrontWheel(WheelID id) => (id == WheelID.LeftFront) || (id == WheelID.RightFront);

            #endregion Brake Component

            #region ABS
            float ABSAdjustedBrakeToqrue(IABS abs, float longitudinalSlipRatio, float brakeTorque)
            {
                // Is the ABS active ? 
                abs.IsActive = Mathf.Abs(longitudinalSlipRatio) > absTargetSlipRatio && brakeTorque > 0f;
                wheelABSState[abs.id] = abs.IsActive;                                
                if (!abs.IsActive) return brakeTorque;

                // Calculate strength of the ABS system,The more the slip ratio overshoots the more brake torque we reduce 
                var absStrength = 1f - MathHelper.MapAndClamp(SlipDelta(longitudinalSlipRatio, absTargetSlipRatio), absTargetSlipRatio, 1f, minABSTorqueRatio, 1f);
                var absBrakeTorque = brakeTorque * absStrength;
                return absBrakeTorque;
            }

            // The strength of of the ABS system acting on the brakes
            float SlipDelta(float longitudinalSlipRatio,float absTargetSlipRatio) => absTargetSlipRatio - longitudinalSlipRatio;            

            #endregion ABS

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Brake;

            public void Start()
            {
                this.maxBrakeTorque.Value = this.config.MaxBrakeforce;
                this.brakeBalance.Value = this.config.brakeBalance;
                this.ABSEnabled.Value = this.config.ABSEnabled;
            }

            public void Shutdown()
            {

            }

            public void Step(BrakeComponenetStepParameters parameters)
            {
            }

            public void UpdateVisuals(float dt)
            {
                // Calculate if the ABS system is activated
                ABSActive.Value = wheelABSState[WheelID.LeftFront] || wheelABSState[WheelID.RightFront] || wheelABSState[WheelID.LeftRear] || wheelABSState[WheelID.RightRear];
            }
            #endregion IVehicleComponent

            #region IDebugInformation
            public void DrawGizmos()
            {
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                return yOffset;
            }
            #endregion IDebugInfomration
        }
        #region Brake Componenet Step Parameters
        public class BrakeComponenetStepParameters
        { }

        #endregion Brake Componenet Step Parameters
    }
}