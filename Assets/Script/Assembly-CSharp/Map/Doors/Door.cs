using System;
using SCPCB.Remaster.Audio;
using SCPCB.Remaster.Map;
using UnityEngine;

namespace SCPCB.Remaster.Door {
	[RequireComponent( typeof( Animator ) )]
	[RequireComponent( typeof( AudioSource ) )]
	public class Door : MonoBehaviour {
		private static readonly int OPEN  = Animator.StringToHash( "Open" );
		private static readonly int CLOSE = Animator.StringToHash( "Close" );

		[SerializeField]
		[Tooltip( "Determines if the door is open or not." )]
		private bool doorOpen;

		[SerializeField]
		[Tooltip( "The location of the buttons." )]
		private Button[] buttons;

		private Animator    animator;
		private AudioSource source;

		public void ToggleDoor() {
			if ( animator.GetCurrentAnimatorStateInfo( 0 ).normalizedTime < 1f ) {
				return;
			}

			doorOpen = !doorOpen;

			animator.SetTrigger( doorOpen ? OPEN : CLOSE );

			source.clip = AudioManager.Singleton["Door"][doorOpen ? "DoorOpen1" : "DoorClose1"].Clip;

			source.Play();
		}

		private void Start() {
			animator = GetComponent<Animator>();
			source   = GetComponent<AudioSource>();

			if ( doorOpen ) {
				animator.SetTrigger( OPEN );
			}
		}
	}
}
