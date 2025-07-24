using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "ESC", menuName = "Components/ESC SO")]
        public class ElectronicStabilityControlSO: ScriptableObjectBase
        {
            [Tooltip("Enable and Disable ESC")]
            public bool Enabled; 
            [Tooltip("Slide angle to activate ESC (Degrees)")]
            public float ActivateBrakeAngle = 10f;
            [Tooltip("Speed To activate ESC (KMH)")]
            public float ActivateSpeed = 20f;
            [Tooltip("Slide angle where brakes are fully Activated (Degrees)")]
            public float MaxBrakeAngle = 80f;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.ESC;
            }

            public static ElectronicStabilityControlSO CreateDefault()
            {
                var c = new ElectronicStabilityControlSO();
                c.Enabled = true;
                c.ActivateBrakeAngle = 10f;
                c.MaxBrakeAngle = 80f;
                c.ActivateSpeed = 20f;                
                return c;
            }
        }
    }
}