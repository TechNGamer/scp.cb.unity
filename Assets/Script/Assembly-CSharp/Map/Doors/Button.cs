using System.Diagnostics.CodeAnalysis;
using SCPCB.Remaster.Audio;
using SCPCB.Remaster.Map;
using UnityEngine;
using UnityEngine.Events;

namespace SCPCB.Remaster.Door {
	[RequireComponent(typeof(AudioSource))]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class Button : MonoBehaviour, IInteractable {

		// Disabling this for the reason that this boolean must be toggleable/saved via Unity.
		// ReSharper disable once ConvertToAutoProperty
		public bool Interactable {
			get => _interactable;
			set => _interactable = value;
		}

		// Using the underscore to denote that the field should not be used directly.
		// ReSharper disable once InconsistentNaming
		[SerializeField]
		[Tooltip("Denotes if the button can be pressed or not.")]
		private bool _interactable;

		[SerializeField]
		private UnityEvent buttonPressed;

		private AudioSource  source;
		private AudioManager audioManager; 

		private void Start() {
			audioManager = AudioManager.Singleton;

			source = GetComponent<AudioSource>();
		}

		public virtual void Interact() {
			if ( !Interactable || buttonPressed == null ) {
				source.clip = audioManager["Interact"]["ButtonFailed"].Clip;
			} else {
				buttonPressed?.Invoke();
			
				source.clip = audioManager["Interact"]["ButtonSuccess"].Clip;
			}

			source.Play();
		}
	}
}
