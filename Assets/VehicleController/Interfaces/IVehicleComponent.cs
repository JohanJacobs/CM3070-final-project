namespace vc
{
    public interface IVehicleComponent
    {
        public ComponentTypes GetComponentType();
        public void Start();
        public void Shutdown();
        public void Update(float dt);
    }

}