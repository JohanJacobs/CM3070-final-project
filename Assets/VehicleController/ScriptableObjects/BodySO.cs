using UnityEngine;

namespace vc
{
    namespace VehicleComponentsSO
    {
        [CreateAssetMenu(fileName = "Body", menuName = "Components/Body SO")]
        public class BodySO : ScriptableObjectBase
        {
            public float turnRadius; // meter
            public float wheelBaseLength; // meter
            public float wheelBaseRearTrackLength; // meter
            public float coefficientOfDrag;
            public float mass; // KG

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