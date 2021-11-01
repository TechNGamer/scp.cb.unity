using UnityEngine;

namespace SCPCB.Remaster.NPC.Guard {
	public class HandingPaperState : StateMachineBehaviour {
		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex ) {
			// TODO: Spawn paper in that is going to be handed to the player.
		}
	}
}
