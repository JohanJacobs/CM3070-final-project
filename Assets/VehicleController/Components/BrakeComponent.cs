using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class BrakeComponent : IVehicleComponent, IDebugInformation
        {
            BrakeSO config;
            FloatVariable brakeInput;
            FloatVariable handbrakeInput;
            float maxBrakeTorque;
            float brakeBalance;
            #region Brake Component
            public BrakeComponent(BrakeSO brakeConfig)
            {
                this.config = brakeConfig;
            }

            public float rearBrakeTorque=> brakeInput.Value * maxBrakeTorque * brakeBalance; // nm
            public float frontBrakeTorque => Mathf.Min(brakeInput.Value * maxBrakeTorque * (1f - brakeBalance) + handbrakeInput.Value * maxBrakeTorque, maxBrakeTorque); //nm
            #endregion Brake Component

            #region IVehicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Brake;

            public void Start()
            {
                this.brakeInput = this.config.brakeInputVariable;
                this.handbrakeInput = this.config.handbrakeInputVariable;
                this.maxBrakeTorque = this.config.MaxBrakeforce;
                this.brakeBalance = this.config.brakeBalance;
            }

            public void Shutdown()
            {

            }

            public void Update(float dt)
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
    }
}