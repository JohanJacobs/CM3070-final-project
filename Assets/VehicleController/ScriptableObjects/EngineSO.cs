using System;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Engine", menuName = "Components/Engine SO")]
        public class EngineSO: ScriptableObjectBase
        {
            [Header("Input")]
            public FloatVariable throttleVariable;

            [ Header("Variables")]
            public FloatVariable RPM;
            public FloatVariable EngineTorque;            

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