using System;
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
		[Tooltip( "The button type to use for this kind of door." )]
		private GameObject button;

		[SerializeField]
		[Tooltip( "The location of the buttons." )]
		private Transform[] buttonLocations;

		private Animator animator;

		private void Start() {
			animator = GetComponent<Animator>();

			InitializeButtons();

			if ( doorOpen ) {
				animator.SetTrigger( OPEN );
			}
		}

		private void InitializeButtons() {
			if ( !button ) {
				/* Doing this to free up memory.
				 * My reasoning is that if there are a ton of buttons,
				 * then a good amount of memory can be freed up.
				 * I do not want to have this game using memory when it shouldn't. */
				foreach ( var loc in buttonLocations ) {
					if ( !loc.GetComponent<Button>() ) {
						Destroy( loc.gameObject );
					}
				}

				buttonLocations = null;

				return;
			}

			// Checking if there is a location to place button.
			// It is also checking to see if a button is already at that transform, and if so,
			// to not delete the transform is not going to use it, otherwise to just go ahead and use it.
			foreach ( var location in buttonLocations ) {
				if ( !location.TryGetComponent( out Button locButton ) ) {
					locButton = Instantiate( button, location ).GetComponent<Button>();
				}
				
				locButton.ButtonPressed += ToggleDoor;
			}
		}

		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			if ( buttonLocations == null ) {
				return;
			}
			
			foreach ( var location in buttonLocations ) {
				if ( location ) {
					Gizmos.DrawSphere( location.position, 0.125f );
				}
			}
		}
		#endif

		private void ToggleDoor() {
			doorOpen = !doorOpen;

			animator.SetTrigger( doorOpen ? OPEN : CLOSE );
		}
	}
}
