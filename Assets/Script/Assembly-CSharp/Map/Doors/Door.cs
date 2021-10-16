using System;
using SCPCB.Remaster.Map;
using UnityEngine;

namespace SCPCB.Remaster.Door {
	[RequireComponent( typeof( Animator ) )]
	public class Door : MonoBehaviour {
		private static readonly int OPEN  = Animator.StringToHash( "Open" );
		private static readonly int CLOSE = Animator.StringToHash( "Close" );

		[SerializeField]
		[Tooltip( "Determines if the door is open or not." )]
		private bool doorOpen;

		[SerializeField]
		[Tooltip( "The location of the buttons." )]
		private Button[] buttons;

		private Animator animator;

		public void ToggleDoor() {
			doorOpen = !doorOpen;

			animator.SetTrigger( doorOpen ? OPEN : CLOSE );
		}

		private void Start() {
			animator = GetComponent<Animator>();

			if ( doorOpen ) {
				animator.SetTrigger( OPEN );
			}
		}
	}
}
