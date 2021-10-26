using System.Diagnostics.CodeAnalysis;
using SCPCB.Remaster.Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCPCB.Remaster.Door {
	[RequireComponent( typeof( Animator ) )]
	[RequireComponent( typeof( AudioSource ) )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class Door : MonoBehaviour {
		private static readonly int OPEN  = Animator.StringToHash( "Open" );
		private static readonly int CLOSE = Animator.StringToHash( "Close" );

		public bool Open {
			get => _open;
			protected set => _open = value;
		}

		public bool IsMoving => animator.GetCurrentAnimatorStateInfo( 0 ).normalizedTime < 1f;

		[FormerlySerializedAs( "doorOpen" )]
		[SerializeField]
		[Tooltip( "Determines if the door is open or not." )]
		// ReSharper disable once InconsistentNaming
		private bool _open;

		protected Animator    animator;
		protected AudioSource source;

		public virtual void ToggleDoor() {
			if ( IsMoving ) {
				return;
			}

			if ( Debug.isDebugBuild ) {
				Debug.Log( "Opening door.", this );
			}

			Open = !Open;

			animator.SetTrigger( Open ? OPEN : CLOSE );

			source.clip = AudioManager.Singleton["Door"][Open ? "DoorOpen1" : "DoorClose1"].Clip;

			source.Play();
		}

		private void Start() {
			animator = GetComponent<Animator>();
			source   = GetComponent<AudioSource>();

			if ( Open ) {
				animator.SetTrigger( OPEN );
			}
		}
	}
}
