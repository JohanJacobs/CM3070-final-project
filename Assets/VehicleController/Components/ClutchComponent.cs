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