using SCPCB.Remaster.Player;
using UnityEngine;

// This class is used to make SCP-106 hunt the player.
public class HuntingState : StateMachineBehaviour {
	private static readonly int STOP_HUNTING = Animator.StringToHash( "StopHunting" );

	private PlayerController player;

	public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex ) {
		player = PlayerController.Player;
	}

	public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex ) {
		if ( player == null || !player ) {
			animator.SetTrigger( STOP_HUNTING );

			return;
		}
		
		animator.transform.LookAt( player.transform );
	}
}
