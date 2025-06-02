using System;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Engine", menuName = "Components/Engine SO")]
        public class EngineSO: ScriptableObjectBase
        {
            [Range(0f,0.5f)]
            public float inertia; // kg/m2
            public float minRPM;
            public float maxRPM;
            public AnimationCurve torqueCurve; // 0->1


            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Engine;
            }
            public static EngineSO CreateDefault()
            {
                var engine = new EngineSO();
                engine.inertia = 0.1f;
                engine.minRPM = 800f;
                engine.maxRPM = 7500f;
                engine.torqueCurve = new AnimationCurve();
                engine.torqueCurve.AddKey(new Keyframe(0.0f, 200f));
                engine.torqueCurve.AddKey(new Keyframe(0.5f, 210f));
                engine.torqueCurve.AddKey(new Keyframe(0.75f, 220f));
                engine.torqueCurve.AddKey(new Keyframe(1f, 180f));

                return engine;
            }
        }
    }
}