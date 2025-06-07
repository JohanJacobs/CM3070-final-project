
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
            
            public float longitudinalSlip; // SX 
            public float lateralSlip;      // SY

            public Vector3 linearVelocityLS; // LocalSpace
            public Vector3 linearVelocityWS; // worldSpace

            // transform for suspension
            public Transform mountPoint;

            // hit point 
            public Vector3 hitPoint;
            public Vector3 hitNormal;   
            public float distanceToWheelAxle;

        }
    }
}