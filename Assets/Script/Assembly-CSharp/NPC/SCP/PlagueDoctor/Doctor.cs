using System;
using SCPCB.Remaster.Player;
using UnityEngine;

namespace SCPCB.Remaster.NPC.SCP.PlagueDoctor {
	[RequireComponent( typeof( Animator ) )]
	public class Doctor : MonoBehaviour {
		private PlayerController player;
		private Animator         anim;

		private void Start() {
			player = PlayerController.Player;
			anim   = GetComponent<Animator>();
		}

		private void Update() {
		}
	}
}
