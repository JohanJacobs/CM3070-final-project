using vc.VehicleComponent;

namespace vc
{
    public interface IVehicleComponent<ParamsType>
    {
        // The component type
        public ComponentTypes GetComponentType();

        // Method that called when a component is created 
        public void Start();

        // Method that called to stop a component 
        public void Shutdown();

        // Standard method of a physics step of a component 
        // during fixed update.
        public void Step(ParamsType parameters);

        // non-fixed update method to update the 
        // visuals for the component each frame
        public void UpdateVisuals(float dt)
        {

        }
    }

}