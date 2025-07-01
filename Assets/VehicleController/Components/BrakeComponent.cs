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
            float maxBrakeTorque;
            float brakeBalance;
            #region Brake Component
            public BrakeComponent(BrakeSO brakeConfig, VehicleVariablesSO variables)
            {
                this.config = brakeConfig;

                // setup variables
                this.brakeInput = variables.brake;
                this.handbrakeInput = variables.handBrake;
            }

            public float rearBrakeTorque=> brakeInput.Value * maxBrakeTorque * brakeBalance; // nm
            public float frontBrakeTorque => Mathf.Min(brakeInput.Value * maxBrakeTorque * (1f - brakeBalance) + handbrakeInput.Value * maxBrakeTorque, maxBrakeTorque); //nm
            #endregion Brake Component

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Brake;

            public void Start()
            {
                this.maxBrakeTorque = this.config.MaxBrakeforce;
                this.brakeBalance = this.config.brakeBalance;
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