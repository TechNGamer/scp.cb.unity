using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPCB.Remaster.Door {
	[RequireComponent( typeof( Animator ) )]
	public class Door : MonoBehaviour {
		[SerializeField]
		[Tooltip("Whether to use the location or not.")]
		private bool useLocation0;
		[SerializeField]
		[Tooltip("Whether to use the location or not.")]
		private bool useLocation1;
		
		[SerializeField]
		[Tooltip("The button type to use for this kind of door.")]
		private GameObject button;

		[SerializeField]
		[Tooltip("The location of one of the buttons.")]
		private Transform button0Location;

		[SerializeField]
		[Tooltip("The location of one of the buttons.")]
		private Transform button1Location;

		private Animator animator;

		private void Start() {
			animator = GetComponent<Animator>();

			if ( !button ) {
				// Doing this to free up memory, even though it is a small amount of memory.
				Destroy( button0Location );
				Destroy( button1Location );

				return;
			}

			// Checking if there is a location to place button.
			if ( button0Location ) {
				if ( useLocation0 ) {
					Instantiate( button, button0Location );
				} else {
					Destroy( button0Location );
				}
			}

			// Disabling in case more is to be added here.
			// ReSharper disable once InvertIf
			if ( button1Location && useLocation1 ) {
				if ( useLocation1 ) {
					Instantiate( button, button1Location );
				} else {
					Destroy( button1Location );
				}
			}
		}

		// Update is called once per frame
		private void Update() {
		}
	}
}
