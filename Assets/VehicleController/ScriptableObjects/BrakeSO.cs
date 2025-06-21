using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Brake", menuName = "Components/Brake SO")]
        public class BrakeSO : ScriptableObjectBase
        {                        
            public float MaxBrakeforce;
            [Range(0f,1f),Tooltip("% to the front, 0.5 middle")]
            public float brakeBalance;

            [Header("Input")]
            public FloatVariable brakeInputVariable;
            public FloatVariable handbrakeInputVariable;

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Brake;
            }

            public static BrakeSO CreateDefault()
            {
                var brake = new BrakeSO();
                brake.brakeBalance = 0.5f;
                brake.MaxBrakeforce = 2000f;
                return brake;
            }
        }
    }
}