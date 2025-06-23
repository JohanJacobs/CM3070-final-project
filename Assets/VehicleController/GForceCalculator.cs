using UnityEngine;

namespace vc
{
    public class GForce
    {
        Vector3 pVelo; // previous velocity measurement in meters per second
        Vector3 cVelo; // current velocity measurement in meters per second
        public Vector3 gForce { get; private set; }
        public float longGForce => gForce.z;
        public float latGForce => gForce.x;

        public Vector3 updateG(Vector3 veloLS_MS, float dt)
        {
            this.pVelo = this.cVelo;
            this.cVelo = veloLS_MS;
            var acceleration = (cVelo - pVelo) / dt;
            gForce = acceleration / 9.81f;
            
            return gForce;

        }
    }

}