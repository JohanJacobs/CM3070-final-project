using vc.VehicleComponentsSO;

namespace vc
{
    namespace VehicleComponent
    {
        public class DifferentialComponent:IVehicleComponent
        {
            DifferentialSO config;
            public DifferentialComponent(DifferentialSO config)
            {
                this.config = config;
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