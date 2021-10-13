using UnityEngine;

namespace SCPCB.Remaster.Map.Lighting {
	public class GenericLightManipulator : LightManipulator {

		[SerializeField]
		[Tooltip( "A list of lights in the room.")]
		protected Light[] lights;

		// Stuff that isn't going to get serialized by the editor.
		// ReSharper disable once InconsistentNaming
		protected LightSettings[] lightSettings;
		
		protected void Awake() {
			lightSettings = new LightSettings[lights.Length];
		}

		protected void Start() {
			TurnRandomLightsOff();
			RandomLightFlicker();
		}

		private void RandomLightFlicker() {
			var flickers = 0;

			while ( flickers != numFlickerLights ) {
				var index    = Random.Range( 0, lights.Length );
				// ReSharper disable once LocalVariableHidesMember
				var light = lights[index];

				if ( lightSettings[index] == LightSettings.Off ) {
					continue;
				}

				if ( lightSettings[index] == LightSettings.Flicker ) {
					continue;
				}

				var flicker = light.gameObject.AddComponent<LightFlicker>();

				flicker.DimCurve       = dimCurve;
				flicker.FlickRate      = flickRate;
				flicker.RandomAddition = randomInterval;

				++flickers;
			}
		}

		private void TurnRandomLightsOff() {
			var lightsOn = lights.Length;

			while ( lightsOn != maxNumOnLights ) {
				// ReSharper disable once LocalVariableHidesMember
				var light    = lights[Random.Range( 0, lights.Length )];
				var lightObj = light.gameObject;

				if ( !lightObj.activeInHierarchy ) {
					continue;
				}

				lightObj.SetActive( false );

				--lightsOn;

				#if UNITY_EDITOR
				Debug.Log( $"Turning of light {light.name}.", light );
				#endif
			}
		}
	}
}
