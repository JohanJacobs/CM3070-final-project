using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace vc
{
    public class FrictionSurface : MonoBehaviour, IFrictionSurface
    {
        [SerializeField] string nameOfSurface = "Default";
        [SerializeField] IFrictionSurface.SurfaceType type;

        public IFrictionSurface.SurfaceType surfaceType => type;
        public string surfaceName => nameOfSurface;        
    }
}