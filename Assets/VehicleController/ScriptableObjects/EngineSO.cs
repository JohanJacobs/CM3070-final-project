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

            [Header("Input")]
            public FloatVariable throttleVariable;
            [Header("Variables")]
            public FloatVariable idleRPMVariable;
            public FloatVariable redlineRPMVariable;
            public FloatVariable currentRPMVariable;
            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Engine;
            }
            public static EngineSO CreateDefault()
            {
                var engine = new EngineSO();
                engine.throttleVariable = Resources.Load<FloatVariable>("Data/SOVariables/throttle");
                return engine;
            }
        }
    }
}