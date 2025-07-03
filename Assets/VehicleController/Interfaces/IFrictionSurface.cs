using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace vc {
    public interface IFrictionSurface
    {
        public enum SurfaceType{
            Default,
            Asphalt,
            DirtyAsphalt,
            Dirt,
            Grass
        }
        public SurfaceType surfaceType { get; }
        public string surfaceName { get; }
        
        public static IFrictionSurface CreateDefault()
        {
            return new DefaultSurface();
        }

        public class DefaultSurface : IFrictionSurface
        {
            public SurfaceType surfaceType => SurfaceType.Default;
            public string surfaceName => "Default";        
        }
    }
}