using UnityEngine;

namespace SCPCB.Remaster.NPC.SCP.Eric {
	public class IdleStateManager : StateMachineBehaviour {
		private static readonly int IDLE = Animator.StringToHash( "Idle" );

		// OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
		public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex ) {
			if ( stateInfo.length >= 0.95f ) {
				return;
			}

			var nextIdle = Random.Range( 0f, 1f );

			if ( nextIdle > 0.9f ) {
				animator.SetInteger( IDLE, 2 );
			} else if ( nextIdle > 0.8f ) {
				animator.SetInteger( IDLE, 1 );
			} else {
				animator.SetInteger( IDLE, 0 );
			}
		}
	}
}
