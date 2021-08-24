using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPCB.Remaster.Door {
	[RequireComponent( typeof( Animator ) )]
	public class Door : MonoBehaviour {
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

		// Start is called before the first frame update
		private void Start() {
			animator = GetComponent<Animator>();
		}

		// Update is called once per frame
		void Update() {
		}
	}
}
