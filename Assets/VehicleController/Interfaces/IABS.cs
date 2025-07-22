namespace vc
{
    namespace VehicleComponent
    {
        public interface IABS:IWheel
        {
            public bool IsActive { get; set; }
            public float LongitudinalSlipRatio { get; }
        }
    }
}