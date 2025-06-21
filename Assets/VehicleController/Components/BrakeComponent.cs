using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using vc.VehicleComponent;

namespace vc
{
    public class BrakeComponent : IVehicleComponent,IDebugInformation
    {
        FloatVariable brake;
        FloatVariable handBrake;
        WheelHitData  hitData;
        BrakeComponent(FloatVariable brake, FloatVariable handbrake, WheelHitData wheelHitData)
        {

        }

        #region IVehicleComponent
        public ComponentTypes GetComponentType() => ComponentTypes.Brake;

        public void Start()
        {

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