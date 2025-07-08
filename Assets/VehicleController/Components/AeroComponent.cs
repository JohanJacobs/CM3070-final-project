using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using vc.VehicleComponent;
using vc.VehicleComponentsSO;
using vc.VehicleConfiguration;

namespace vc
{
    public class AeroComponent:IVehicleComponent<AeroComponentStepParameters>,IDebugInformation
    {
        AeroSO config;
        Rigidbody rb;

        public FloatVariable DragCoefficient;
        public FloatVariable LiftCoefficient;
        public FloatVariable SurfaceAera;

        float totalDownforce = default; // newton
        float totalDragForce = default; // newton
        
        Vector3 velocityLS =default;
        float aeroVelocity => Mathf.Max(0f, velocityLS.z); // aero don't work in reverse speeds
        Vector3 dragDirection => -rb.transform.forward;
        Vector3 downforceDirection => -rb.transform.up;

        #region AeroComponent
        public AeroComponent(AeroSO config, Rigidbody carRigidbody, VehicleVariablesSO variables)
        {
            this.config = config;
            this.rb = carRigidbody;

            this.DragCoefficient = variables.DragCoefficient;
            this.LiftCoefficient = variables.LiftCoefficient;
            this.SurfaceAera = variables.SurfaceAera;
        }

        private void CalculateAeroForces()
        {
            CalculateDrag();
            CalculateDownforce();
        }

        private void CalculateDrag()
        {         
            totalDragForce += PhysicsHelper.CalculateDrag(aeroVelocity, SurfaceAera.Value, DragCoefficient.Value);
        }

        private void CalculateDownforce()
        {            
            totalDownforce += PhysicsHelper.CalculateLift(aeroVelocity, SurfaceAera.Value, LiftCoefficient.Value);
        }

        private void ApplyAeroForces()
        {
            Vector3 downforce = downforceDirection * totalDownforce;
            Vector3 dragforce = dragDirection * totalDownforce;
            rb.AddForce(downforce + dragforce);
        }
        #endregion AeroComponent


        #region IVehicleComponent
        public void Step(AeroComponentStepParameters parameters)
        {
            totalDragForce = 0f;
            totalDownforce = 0f;
            
            this.velocityLS = parameters.velocityLS;

            CalculateAeroForces();
            ApplyAeroForces();
        }
        public void Start()
        {
            this.DragCoefficient.Value = this.config.dragCoefficient;
            this.LiftCoefficient.Value = this.config.LiftCoefficient;
            this.SurfaceAera.Value = this.config.surfaceArea;
        }
        public void Shutdown()
        {

        }
        public ComponentTypes GetComponentType()
        {
            return ComponentTypes.Aero;
        }

        #endregion IVehicleComponent

        #region IDebugInformation
        public void DrawGizmos()
        {
            
        }

        public float OnGUI(float xOffset, float yOffset, float yStep)
        {

            GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"- AERO:");
            GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Downforce:{PhysicsHelper.Conversions.NewtonToKG(totalDownforce).ToString("F0")} KG");
            GUI.Label(new Rect(xOffset, yOffset += yStep, 200f, yStep), $"  Downforce:{totalDownforce.ToString("F0")} N");

            return yOffset;
        }
        #endregion IDebugInformation

    }

    public class AeroComponentStepParameters {
        public AeroComponentStepParameters(Vector3 veloLS)
        {
            this.velocityLS = veloLS;
        }
        public Vector3 velocityLS;
    }

}
