using System;
using System.Collections;
using System.Collections.Generic;
using SCPCB.Remaster.Player;
using SCPCB.Remaster.Utility;
using UnityEngine;

namespace SCPCB.Remaster.NPC.SCP.PlagueDoctor {
	[RequireComponent( typeof( Animator ) )]
	public class Doctor : MonoBehaviour {
		private Vector3? playerOldPos;

		private PlayerController player;
		private Animator         anim;
		private Coroutine        walkingPath;

		[SerializeField]
		private AStarGridManager gridManager;

		private void Start() {
			player = PlayerController.Player;
			anim   = GetComponent<Animator>();
		}

		private void Update() {
			if ( playerOldPos == null || playerOldPos != player.transform.position ) {
				if ( walkingPath != null ) {
					StopCoroutine( walkingPath );
				}

				var path = gridManager.FindPath( transform.position, player.transform.position );

				walkingPath = StartCoroutine( FollowPath( path ) );

				for ( var i = 0; i < path.Length - 1; ) {
					if ( i == 0 ) {
						Debug.DrawLine( transform.position, path[i].worldPosition, Color.red, float.MaxValue );
					}

					Debug.DrawLine( path[i].worldPosition, path[++i].worldPosition, Color.red, float.MaxValue );
				}

				playerOldPos = player.transform.position;
			}
		}

		private IEnumerator FollowPath( IEnumerable<AStarGridManager.Node> path ) {
			var myTransform = transform;
			
			foreach ( var node in path ) {
				var heading = node.worldPosition - transform.position;

				while ( heading.magnitude > 0.5f ) {
					var distance  = heading.magnitude;
					var direction = heading / distance;
					var position  = myTransform.position;
					
					position             += direction * 1f * Time.deltaTime;
					myTransform.position =  position;

					Debug.DrawRay( position, direction );

					yield return new WaitForEndOfFrame();

					heading = node.worldPosition - position;
				}
			}
		}
	}
}