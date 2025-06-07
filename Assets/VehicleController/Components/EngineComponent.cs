using Unity.VisualScripting;
using UnityEngine.Events;
using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        
        public class EngineComponent : IVehicleComponent
        {
            EngineSO config;
            FloatVariable throttle;
            FloatVariable RPM;
            FloatVariable engineTorque;

            public EngineComponent(EngineSO config)
            {
                this.config = config;
                this.throttle = config.throttleVariable;
                this.RPM = config.RPM;
                this.engineTorque= config.EngineTorque;
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
                engineTorque.Value = throttle.Value * 1f;
            }
            #endregion IVerhicleComponent
        }

    }
}