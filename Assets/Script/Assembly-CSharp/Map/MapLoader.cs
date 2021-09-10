using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPCB.Remaster.Map {
	public class MapLoader : MonoBehaviour {

		[SerializeField]
		private byte gridSize = 10;

		private Vector3[] positions;
		
		[SerializeField]
		[Tooltip("This field will probably get removed later.")]
		private GameObject[] prefabs;

		private async Task Awake() {
			
		}

		private void OnDrawGizmos() {
			if ( positions == null || positions.Length == 0 ) {
				return;
			}

			foreach ( var position in positions ) {
				Gizmos.DrawSphere( position, 0.25f );
			}
		}

		private IEnumerator GenerateMap() {
			const float ROOM_SIZE = 14.3f;
			var         waitTime  = 0.1f;

			positions = new Vector3[gridSize * gridSize];
			
			for (int i = 0, c = -(gridSize / 2); c < gridSize / 2 + gridSize % 2; ++c ) {
				for ( var r = 0; r < gridSize; ++r ) {
					// The plus 1 is meant as an offset since Unity wants this in world space.
					var worldPos = new Vector3( ( r + 1 ) * ROOM_SIZE, 0f, c * ROOM_SIZE );
					var gameObj  = Instantiate( prefabs[0], worldPos, Quaternion.identity, transform );

					positions[i++] = worldPos;

					yield return new WaitForSeconds( waitTime );
				}
			}
			
			Debug.Log( "Done generating map.", this );
		}
	}
}
