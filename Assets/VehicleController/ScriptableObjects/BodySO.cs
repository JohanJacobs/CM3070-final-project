using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Body", menuName = "Components/Body SO")]
        public class BodySO : ScriptableObjectBase
        {
            public FloatVariable steerVariable;
            public FloatVariable speedKMHVariable;

            public float turnRadius;
            public float wheelBaseLength;
            public float wheelBaseRearTrackLength;


            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Body;
            }

            public static BodySO CreateDefault()
            {
                var body = new BodySO();
                body.turnRadius = 9.3f;
                body.wheelBaseLength = 2.86f;
                body.wheelBaseRearTrackLength = 1.69f;

                return body;
            }
        }
    }
}