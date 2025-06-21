using UnityEngine;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {

        public class ClutchComponent : IVehicleComponent
        {
            ClutchSO config;               
            FloatVariable engineTorque;

            public ClutchComponent(ClutchSO config)
            {
                this.config = config;
                this.engineTorque = config.engineTorque;
            }

            float clutchCapacity = 1.3f;
            float clutchStiffness = 40f;
            float clutchDamping = 0.7f;

            float engineMaxTorque = 400f; // nm TODO: read from engine
            float engineMaxRPM = 7000f;   // rmp TODO: read from engine 
            float clutchMaxTorque => clutchCapacity * clutchMaxTorque;

            float RadToRPM => (2 * Mathf.PI) / 60f;
            float engineRPMRate=> (engineAngularVelocity*RadToRPM) / engineMaxRPM;
            float NeutralGearCheck => transmissionRatio < float.Epsilon ? 1 : 0f;
            float clutchLock => Mathf.Min(engineRPMRate + NeutralGearCheck, 1f);
            float clutchSlip => (engineAngularVelocity - clutchAngularVelocity) * MathHelper.Sign(Mathf.Abs(transmissionRatio));
            
            float transmissionVelocity = default;
            float transmissionRatio = default;
            float engineAngularVelocity = default;
            float clutchTorque = default;
            float clutchAngularVelocity=default;

            public void Update(float transmissionVelocity, float transmissionRatio, float engineAngularVelocity)
            {
                // clutch Damping
            
                var transferredClutchTorque = Mathf.Clamp(clutchSlip * clutchLock * clutchStiffness, -clutchMaxTorque, clutchMaxTorque);
                var damping = (transferredClutchTorque - clutchTorque) * clutchDamping;
                clutchTorque = transferredClutchTorque + damping;
            }

            #region IVerhicleComponent
            public ComponentTypes GetComponentType() => ComponentTypes.Differential;
            public void Shutdown()
            {

            }

            public void Start()
            {

            }

            public void Update(float dt)
            {
            }
            #endregion IVerhicleComponent
        }

    }
}