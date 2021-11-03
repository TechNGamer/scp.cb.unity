using System;
using UnityEngine;

namespace SCPCB.Remaster.NPC {
	// This state object is used to rotate the Generic Humanoid to the correct position before the animation plays.
	public class GettingUpState : StateMachineBehaviour {
		private static readonly int HIT_DIR = Animator.StringToHash( "HitDir" );

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex ) {
			var direction  = animator.GetInteger( HIT_DIR );
			var gameObject = animator.gameObject;

			switch ( direction ) {
				case 1:
					throw new Exception( "This animation should not be playing for falling forward." );
				case 3:
					break;
				// Animation of falling right.
				case 0:
					gameObject.transform.Rotate( Vector3.up, -90f );
					break;
				// Animation of falling left.
				case 2:
					gameObject.transform.Rotate( Vector3.up, 90f );
					break;
				default:
					throw new Exception( $"Expected {nameof( direction )} to be within (0,3)." );
			}
		}
	}
}
