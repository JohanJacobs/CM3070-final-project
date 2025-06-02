using UnityEngine;


namespace vc
{
	namespace VehicleComponentsSO
    {
		[CreateAssetMenu(fileName = "Suspension", menuName = "Components/Suspension SO")]
		public class SuspensionSO: ScriptableObjectBase
		{
			public float SuspensionStrength;
			public float DamperStrength;

			[Tooltip("In Centimeter")]
			public float RestLength; // centimeter


			public float RestLengthMeter => RestLength/100f; // meter
				 

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Suspension;
            }


            public static SuspensionSO CreateDefault()
			{
				var susp = new SuspensionSO();
				susp.SuspensionStrength = 7000f;
				susp.DamperStrength = 700f;
				susp.RestLength = 50f;

				return susp;
			}

        }
	}
}