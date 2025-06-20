using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class TransmissionComponent : IVehicleComponent
        {
            TransmissionSO config;
            public TransmissionComponent(TransmissionSO config)
            {
                this.config = config;
            }

            #region IVehicleComponent
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
            #endregion IVehicleComponent
        }

    }
}