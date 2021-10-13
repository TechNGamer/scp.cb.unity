using System.Diagnostics.CodeAnalysis;
using SCPCB.Remaster.Player;
using UnityEngine;

namespace SCPCB.Remaster.Map.Lighting {
	// ReSharper disable once InconsistentNaming
	[SuppressMessage( "ReSharper", "LocalVariableHidesMember" )]
	public class SCP1123LightManager : LightManipulator {
		[SerializeField]
		[Tooltip( "The tag for the player." )]
		private string playerTag = "Player";

		[SerializeField]
		[Tooltip( "The lights that are on the first floor." )]
		private Light[] firstFloorLights;

		[SerializeField]
		[Tooltip( "The lights that are on the second floor." )]
		private Light[] secondFloorLights;

		private bool firstLightsOn = true;
		
		private PlayerController player;

		private void Start() {
			player = PlayerController.Player;

			// Ensures that the memetic floor lights get's disabled.
			foreach ( var light in secondFloorLights ) {
				light.gameObject.SetActive( false );
			}
		}

		private void FixedUpdate() {
			if ( player.transform.position.y > 3f && firstLightsOn ) {
				ToggleLights();

				firstLightsOn = false;
			} else if ( player.transform.position.y < 3f && !firstLightsOn ) {
				ToggleLights();

				firstLightsOn = true;
			}
		}

		private void ToggleLights() {
			foreach ( var light in firstFloorLights ) {
				light.gameObject.SetActive( !light.isActiveAndEnabled );
			}

			foreach ( var light in secondFloorLights ) {
				light.gameObject.SetActive( !light.isActiveAndEnabled );
			}
		}
	}
}
