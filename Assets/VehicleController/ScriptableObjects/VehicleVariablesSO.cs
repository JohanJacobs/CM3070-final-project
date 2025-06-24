using UnityEngine;

namespace vc
{
    namespace VehicleConfiguration
    {
        [CreateAssetMenu(fileName = "VehicleVariables", menuName = "Vehicle Variable SO")]
        public class VehicleVariablesSO: ScriptableObjectBase
        {
            [Header("General")]
            public FloatVariable speedKMH;

            [Header("Engine")]
            public FloatVariable currentRPM;
            public FloatVariable maxRPM;
            public FloatVariable minRPM;

            [Header("Transmission")]
            public StringVariable currentGearText;

        }
    }
}