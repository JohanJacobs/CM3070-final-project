
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using vc.VehicleConfiguration;

namespace vc
{
    namespace VehicleComponent
    {
        /*
            SAE convetions 
            SAE X = Unity Z 
            SAE Y = Unity X
            SAE Z = Unity Y         
         */

        public class WheelHitData
        {
            public WheelComponent wheel;
            public SuspensionComponent suspension;
            public BodyComponent body;
            public Rigidbody rb;
            public RaycastHit hitInfo;
            // ID 
            public WheelID id;

            public Vector3 velocityLS; // Meter per Second


            // Friction Surface
            public IFrictionSurface.SurfaceType FrictionSurfaceType => surface.surfaceType;
            public string FrictionSurfaceName => surface.surfaceName;
            public IFrictionSurface surface;

            public float normalforce => suspension.normalForce;        
            public float longitudinalSlipRatio => wheel?.LongitudinalSlipRatio ?? 0f;
            public bool isLocked => longitudinalSlipRatio >= 1f;
            public float springCompression => suspension?.springCompression ?? 0f;
            public float SpeedMS => velocityLS.z;
            public float SpeedKMH=> SpeedMS / 3.6f;
            public bool isGrounded => suspension.isGrounded;
            public Vector3 axlePosition => suspension.axlePosition;
            public Vector3 forward => suspension.mountPoint.forward;
            public Vector3 right => suspension.mountPoint.right;
            public Transform suspensionMountPoint => suspension.mountPoint;
            public static Dictionary<WheelID, WheelHitData> SetupDefault(Rigidbody carRigidbody)
            {
                Dictionary<WheelID, WheelHitData> wheelHitData = new();
                wheelHitData.Add(WheelID.LeftFront, new WheelHitData
                {
                    id = WheelID.LeftFront,
                    rb = carRigidbody,
                });


                wheelHitData.Add(WheelID.RightFront, new WheelHitData
                {
                    id = WheelID.RightFront,
                    rb = carRigidbody,
                });

                wheelHitData.Add(WheelID.LeftRear, new WheelHitData
                {
                    id = WheelID.LeftRear,
                    rb = carRigidbody,
                });


                wheelHitData.Add(WheelID.RightRear, new WheelHitData
                {
                    id = WheelID.RightRear,
                    rb = carRigidbody,
                });

                return wheelHitData;
            }
        }
    }
}