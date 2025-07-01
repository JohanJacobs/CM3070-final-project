using System;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Engine", menuName = "Components/Engine SO")]
        public class EngineSO: ScriptableObjectBase
        {
            [Header("Configuration")]
            public float redlineRPM;
            public float idleRPM;
            public AnimationCurve torqueCurve;

            public float startFriction;
            public float engineEnirtia;
            public float frictionCoefficient;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Engine;
            }
            public static EngineSO CreateDefault()
            {
                var engine = new EngineSO();
                return engine;
            }
        }
    }
}