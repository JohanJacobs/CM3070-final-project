namespace vc
{
    namespace VehicleComponent {
        public interface IDebugInformation
        {

            public void DrawGizmos();
            public float OnGUI(float xOffset, float yOffset, float yStep);
        }
    }

}