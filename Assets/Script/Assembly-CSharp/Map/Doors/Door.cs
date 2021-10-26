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

			Open = !Open;

			animator.SetTrigger( Open ? OPEN : CLOSE );

			var rand = Random.Range( 1, 4 );
			var clip = Open ? $"DoorOpen{rand}" : $"DoorClose{rand}";

			source.clip = AudioManager.Singleton["Door"][clip].Clip;

			source.Play();

			if ( Debug.isDebugBuild ) {
				Debug.Log( $"Playing `{clip}`." );
			}
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
