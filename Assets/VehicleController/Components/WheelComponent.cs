using System.Diagnostics;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class WheelComponent : IVehicleComponent,IDebugInformation
        {

            WheelSO config;
            WheelHitData wheelHitData;

            public WheelID id { get; private set; }
            
            public float radius{ get; private set; }   
            

            public WheelComponent(WheelSO config, WheelID id)
            {
                this.config = config;
                this.id = id;
                this.radius = config.RadiusMeter;
            }

            public void SetWheelHitData(WheelHitData wheelHitData)
            {
                this.wheelHitData = wheelHitData;
            }

            #region IVehicleComponent
            public ComponentTypes GetComponentType()
            {
                return ComponentTypes.Wheel;
            }

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

            #region IDebugInformation
            public void DrawGizmos()
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(this.wheelHitData.hitPoint, this.wheelHitData.hitNormal * radius);

            }
            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                return yOffset;
            }

            #endregion IDebugInformation
        }
    }
}