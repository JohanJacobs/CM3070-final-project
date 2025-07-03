using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                        
            public WheelSurfaceProperties[] frictionProperties;

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

        [System.Serializable]
        public class WheelSurfaceProperties {
            //https://hpwizard.com/tire-friction-coefficient.html
            public IFrictionSurface.SurfaceType surfaceType = IFrictionSurface.SurfaceType.Default;
            [HorizontalGroup("FrictionValues"), Tooltip("Friction coefficient on dry surface")]
            public float Dry=1f;
            [HorizontalGroup("FrictionValues"),Tooltip("Friction coefficient on wet surface")]
            public float Wet=0.5f;
            [HorizontalGroup("FrictionValues"),Tooltip("Rolling Resistance")]
            public float RR = 0.01f;

            public static Dictionary<IFrictionSurface.SurfaceType, WheelSurfaceProperties>  CreateDictionary(WheelSurfaceProperties[] properties)
            {
                var dict = new Dictionary<IFrictionSurface.SurfaceType, WheelSurfaceProperties>();

                foreach (var st in properties)
                {
                    // duplicate entry, only use the first one
                    if (dict.ContainsKey(st.surfaceType))
                        continue;

                    dict.Add(st.surfaceType, st);
                }

                // make sure there is a default surface type value
                if (!dict.ContainsKey(IFrictionSurface.SurfaceType.Default))
                {
                    dict.Add(IFrictionSurface.SurfaceType.Default, new WheelSurfaceProperties());
                }
                return dict;
            }
        }
    }
}