using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;


namespace vc
{
	namespace VehicleComponentsSO
    {
		[CreateAssetMenu(fileName = "Suspension", menuName = "Components/Suspension SO")]
		public class SuspensionSO: ScriptableObjectBase
		{
			[Unit(Units.Newton)]
			public float suspensionForce;
            [Unit(Units.Newton)]
            public float DamperStrength;

			[Tooltip("In Centimeter"),Unit(Units.Centimeter)]
			public float RestLength; // centimeter

			public float RestLengthMeter => RestLength/100f; // meter

            public static ComponentTypes GetVehicleComponentType()
            {
                return ComponentTypes.Suspension;
            }


            public static SuspensionSO CreateDefault()
			{
				var susp = new SuspensionSO();
				susp.suspensionForce = 2.6f;
				susp.DamperStrength = 700f;
				susp.RestLength = 50f;
				

				return susp;
			}

        }
	}
}