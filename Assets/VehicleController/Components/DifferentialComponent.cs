using System.Collections.Generic;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class DifferentialComponent:IVehicleComponent,IDebugInformation
        {
            DifferentialSO config;
            int connectedWheels;
            float ratio;
            float[] wheelOutput;
            public DifferentialComponent(DifferentialSO config, int connectedWheels)
            {
                this.config = config;
                ratio = config.GearRatio;
                this.connectedWheels = connectedWheels;
                this.wheelOutput = new float[connectedWheels];
            }

            #region Differential Component            
            public float[] CalculateOutputTorque(float transmissionTorque) 
            {
                var totalTorque = transmissionTorque * ratio;
                float torque = MathHelper.SafeDivide(totalTorque, (float)connectedWheels);

                wheelOutput[0] = torque;
                wheelOutput[1] = torque;                
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

            public void DrawGizmos()
            {
                
            }

            public float OnGUI(float xOffset, float yOffset, float yStep)
            {
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"DIFFERENTIAL");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Ratio : {ratio}");
                GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  wheelOutput: {wheelOutput[0]}");

                return yOffset;
            }
            #endregion IVerhicleComponent
        }

    }
}