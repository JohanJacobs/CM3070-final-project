
using UnityEngine;

namespace vc
{
    namespace VehicleComponent
    {
        public class WheelHitData
        {
            // state 
            public bool isGrounded;

            // physics 
            public Rigidbody rb;
            public float normalForce;

            // hit point 
            public Vector3 hitPoint;
            public Vector3 hitNormal;   
            public float distanceToWheelAxle;

        }
    }
}