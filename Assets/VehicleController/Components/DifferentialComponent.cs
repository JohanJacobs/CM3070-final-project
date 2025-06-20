using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class DifferentialComponent:IVehicleComponent
        {
            DifferentialSO config;
            int connectedWheels;
            float ratio;
            public DifferentialComponent(DifferentialSO config, int connectedWheels)
            {
                this.config = config;
                ratio = config.GearRatio;
                this.connectedWheels = connectedWheels;
            }

            #region Differential Component
            public float[] CalculateOutputTorque(float transmissionTorque) 
            {   
                float torque = MathHelper.SafeDivide(transmissionTorque * ratio, (float)connectedWheels);

                float[] wheelOutput = { torque, torque };
                return wheelOutput;
            }


            #endregion Differential Component


            #region IVerhicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Differential;
            public void Shutdown()
            {
                
            }

            public void Start()
            {
                
            }

            public void Update(float dt)
            {
                
            }
            #endregion IVerhicleComponent
        }

    }
}