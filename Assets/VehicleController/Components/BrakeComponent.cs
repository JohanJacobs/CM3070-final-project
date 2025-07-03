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
        public class BrakeComponent : IVehicleComponent<BrakeComponenetStepParameters>, IDebugInformation
        {
            BrakeSO config;
            FloatVariable brakeInput;
            FloatVariable handbrakeInput;
            FloatVariable maxBrakeTorque;
            FloatVariable brakeBalance;
            #region Brake Component
            public BrakeComponent(BrakeSO brakeConfig, VehicleVariablesSO variables)
            {
                this.config = brakeConfig;

                // setup variables
                this.brakeInput = variables.brake;
                this.handbrakeInput = variables.handBrake;

                this.maxBrakeTorque = variables.brakeTorque;
                this.brakeBalance = variables.brakeBalance;
            }

            public float rearBrakeTorque=> brakeInput.Value * maxBrakeTorque.Value * brakeBalance.Value; // nm
            public float frontBrakeTorque => Mathf.Min(brakeInput.Value * maxBrakeTorque.Value * (1f - brakeBalance.Value) + handbrakeInput.Value * maxBrakeTorque.Value, maxBrakeTorque.Value); //nm
            #endregion Brake Component

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Brake;

            public void Start()
            {
                this.maxBrakeTorque.Value = this.config.MaxBrakeforce;
                this.brakeBalance.Value = this.config.brakeBalance;
            }

            public void Shutdown()
            {

            }

            public void Step(BrakeComponenetStepParameters parameters)
            {

            }
            #endregion IVehicleComponent

            #region IDebugInfomration
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