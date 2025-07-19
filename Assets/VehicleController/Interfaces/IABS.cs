namespace vc
{
    namespace VehicleComponent
    {
        public interface IABS
        {
            public WheelID id { get; }
            bool IsActive { get; set; }
        }
    }
}