using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCPCB.Remaster {
	public class LightManipulator : MonoBehaviour {
		public class LightFlicker : MonoBehaviour {
		}

		[SerializeField]
		[Tooltip( "The number of lights to randomly disable." )]
		[InspectorName("Maximum Lights On")]
		[Range( 1, 6 )]
		private int maxNumOnLights;

		[SerializeField]
		[Tooltip( "Number of lights to flicker." )]
		[Range( 1, 6 )]
		private int numFlickerLights;

		[SerializeField]
		[Tooltip( "A list of lights in the room." )]
		private GameObject[] lights;

		private void Start() {
			StartCoroutine( TurnRandomLightsOff() );
		}

		private IEnumerator TurnRandomLightsOff() {
			var lightsOn = lights.Length;

			while ( lightsOn != maxNumOnLights ) {
				var lightObj = lights[Random.Range( 0, lights.Length )];
				
				lightObj.SetActive( false );

				--lightsOn;

				yield return new WaitForSeconds( 1f );
			}
		}
	}
}
