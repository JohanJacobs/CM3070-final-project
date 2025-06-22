using vc.VehicleComponent;

namespace vc
{
    public interface IVehicleComponent<ParamsType>
    {
        public ComponentTypes GetComponentType();
        public void Start();
        public void Shutdown();
        public void Step(ParamsType parameters);

        //public void Update(float dt);
    }

        
    //public interface ITest<T> {
    //    public void step(T t);
    //}

    //public class TestIt: ITest<TestIt.TestParam>
    //{
    //    public void step(TestParam t)
    //    {
            
    //    }

    //    public class TestParam
    //    {
    //    }
    //}


}