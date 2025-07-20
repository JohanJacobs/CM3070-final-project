using Sirenix.OdinInspector;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "TractionControl", menuName = "Components/TractionControl SO")]
        public class TractionControlSO : ScriptableObjectBase
        {
            public bool TCEnabled = true;

            public float TCMin = 0f;
            public float TCMax = 10f;
            public float TCDefault = 2f;
            public float TCStep = 1f;
            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.TractionControl;
            }

            public static TractionControlSO CreateDefault()
            {
                var tc = new TractionControlSO();

                return tc;
            }
        }
    }
}