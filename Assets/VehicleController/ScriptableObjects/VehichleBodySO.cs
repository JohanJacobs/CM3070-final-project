using System;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "VehichleBody", menuName = "Components/VehichleBody SO")]
        public class VehichleBodySO : ScriptableObjectBase
        {
            [Tooltip("Kilograms")]
            public float weight;
            [Range(0f, 1f)]
            public float CoefficientOfDrag;

            [Tooltip("In meters")]
            public float TurnRadius;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Body;
            }

            public static VehichleBodySO CreateDefault()
            {
                var body = new VehichleBodySO();
                body.weight = 1500f;
                body.CoefficientOfDrag = 0.3f;
                return body;
            }

        }
    }
}