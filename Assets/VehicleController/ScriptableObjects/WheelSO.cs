using UnityEngine;

namespace vc {
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName ="Wheel",menuName = "Components/Wheel SO")]
        public class WheelSO : ScriptableObjectBase
        {
            [Tooltip("In Centimeter")]
            public float Radius;
            [Tooltip("In Kilogram")]
            public float Mass;

            public float RadiusMeter => Radius / 100f; // meters
            public static ComponentTypes GetVehicleComponentType ()
            {
                return ComponentTypes.Wheel;
            }

            public static WheelSO CreateDefault()
            {
                var w = new WheelSO();

                w.Radius = 35;
                w.Mass = 6f;
                return w;
            }
        }
    }
}