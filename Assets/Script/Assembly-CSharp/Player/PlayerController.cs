using System.Diagnostics.CodeAnalysis;
using SCPCB.Remaster.Audio;
using SCPCB.Remaster.Map;
using UnityEngine;

namespace SCPCB.Remaster.Player {
	/// <summary>
	/// The player controller.
	/// </summary>
	[RequireComponent( typeof( AudioSource ) )]
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	public class PlayerController : MonoBehaviour {
		public static PlayerController Player { get; private set; }

		private static readonly Vector3 BOX_SIZE = new Vector3( 0.5f, 0.5f, 0.5f );

		/// <summary>
		/// If the player is touching the ground.
		/// </summary>
		/// <remarks>
		/// This value cannot be set outside the class. It is determined every update.
		/// </remarks>
		public bool IsGrounded { get; private set; }

		/// <summary>
		/// If the player is crouching or not.
		/// </summary>
		/// <remarks>
		/// This value can be changed externally, but might be closed off.
		/// </remarks>
		public bool IsCrouched { get; set; }

		/// <summary>
		/// If the player is running or not.
		/// </summary>
		/// <remarks>
		/// This value is set by the Input System, and should generally not be modified.
		/// </remarks>
		public bool IsRunning { get; private set; }

		[SerializeField]
		[Range( 1f, 100f )]
		[Tooltip( "The speed at which the camera moves." )]
		private float camSpeed = 1f;

		[SerializeField]
		[Range( 1f, 5f )]
		[Tooltip( "The speed at which the player walks." )]
		private float walkSpeed = 1f;

		[SerializeField]
		[Range( 0.5f, 1f )]
		[Tooltip( "The speed at which the player crouch walks." )]
		private float crouchSpeed = 1f;

		[SerializeField]
		[Range( 1.25f, 5f )]
		[Tooltip( "The speed at which the player runs." )]
		private float sprintSpeed = 1f;

		[SerializeField]
		[Range( 0.125f, 1 )]
		[Tooltip( "The distance before snapping to the ground." )]
		private float groundDistance = 0.25f;

		[SerializeField]
		[Range( 30f, 90f )]
		[Tooltip( "The maximum vertical look angle that is allowed." )]
		private float maxLookAngle = 45f;

		private float fallSpeed;

		private Vector3 moveCamRot;
		private Vector3 moveDirection;

		[SerializeField]
		[Tooltip( "The mask for the ground." )]
		private LayerMask groundMask;

		private Camera              cam;
		private CharacterController cc;
		private MainInput           input;
		private AudioSource         aSource;
		private AudioManager        audioManager;

		[SerializeField]
		[Tooltip( "The object where a ground check occurs." )]
		private Transform groundCheck;

		[SerializeField]
		private LayerMask interactMask;

		private          IInteractable interactable;
		private readonly RaycastHit[]  hits = new RaycastHit[16];

		#region Unity Methods
		private void Awake() {
			// Want to make sure that there is only one player controller.
			if ( Player ) {
				Destroy( this );
				return;
			}

			Player       = this;
			audioManager = AudioManager.Singleton;

			aSource = GetComponent<AudioSource>();

			aSource.loop = false;

			#region Input Assigning
			// This section is mainly used to initialize the events of the new Input System.
			input = new MainInput();

			input.Game.Look.performed += ctx => {
				var value = ctx.ReadValue<Vector2>();

				moveCamRot = new Vector3( -value.y, value.x );
			};
			input.Game.Look.canceled += _ => { moveCamRot = Vector3.zero; };

			// TODO: Figure out what is going on here and why Move.canceled isn't being called when the shit key is being held down.
			input.Game.Move.performed += ctx => {
				var value = ctx.ReadValue<Vector2>();

				moveDirection = new Vector3( value.y, 0f, value.x );
			};
			input.Game.Move.canceled += _ => {
				moveDirection = Vector3.zero;
			};

			// The reason to use the underscore (aka, the discard variable) is that we do not care
			// about the context.
			// TODO: Rewrite to enable toggling of the crouch function, and add it to settings.
			input.Game.Crouch.started += _ => {
				if ( IsRunning ) {
					IsRunning = false;
				}

				IsCrouched = true;
			};
			input.Game.Crouch.canceled += _ => { IsCrouched = false; };

			input.Game.Sprint.started += _ => {
				if ( IsCrouched ) {
					return;
				}

				if ( moveDirection == Vector3.zero ) {
					return;
				}

				IsRunning = true;
			};
			input.Game.Sprint.performed += _ => { IsRunning = moveDirection != Vector3.zero; };
			input.Game.Sprint.canceled += _ => {
				IsRunning = false;
			};

			input.Game.Enable();

			input.Game.Interact.started += _ => {
				if ( interactable == null ) {
					return;
				}

				interactable.Interact();
			};
			#endregion
		}

		// Start is called before the first frame update
		private void Start() {
			// Instead of having it be associated within the editor, it will grab it during runtime.
			// This is better in my opinion because if anything changes with the Character PlayerController
			// or the Camera, then it will be picked up during runtime instead of erroring out and forgetting.
			cam = GetComponentInChildren<Camera>();
			cc  = GetComponent<CharacterController>();

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible   = false;
		}

		// Update is called once per frame
		private void Update() {
			MoveCharacter();
			MoveCamera();
			PlayCharacterSounds();
		}

		private void FixedUpdate() {
			var camTransform = cam.transform;
			var forward      = camTransform.forward;
			var checkPos     = forward + camTransform.position;
			var size         = Physics.BoxCastNonAlloc( checkPos, BOX_SIZE, forward, hits );

			if ( size == 0 ) {
				interactable = null;

				return;
			}

			for ( var i = 0; i < size; ++i ) {
				var hit = hits[i];
				
				CalculateInteractable( hit );
			}
		}

		private void OnDrawGizmosSelected() {
			var camTransform = cam.transform;
			var checkPos     = camTransform.forward + camTransform.position;

			Gizmos.color = Color.blue;

			Gizmos.DrawWireCube( checkPos, BOX_SIZE * 2f );
		}
		#endregion

		private void PlayCharacterSounds() {
			if ( moveDirection.x == 0 && moveDirection.z == 0 || aSource.isPlaying ) {
				return;
			}

			aSource.clip = IsRunning ? audioManager["Steps"]["Run1"].Clip : audioManager["Steps"]["Step1"].Clip;

			aSource.Play();
		}

		private void CalculateInteractable( RaycastHit hit ) {
			var camTransform = cam.transform;
			var camPos       = camTransform.position;
			var otherPos     = hit.transform.position;
			var hitObj       = hit.transform.gameObject;

			Debug.DrawLine( camPos, otherPos, Color.cyan );

			var viewedInteractable = hit.transform.gameObject.GetComponent<IInteractable>();

			if ( !( viewedInteractable is MonoBehaviour ) ) {
				if ( !( hitObj.GetComponentInParent<IInteractable>() is MonoBehaviour intMono ) ) {
					return;
				}

				viewedInteractable = ( IInteractable )intMono;
			}


			if ( interactable == null ) {
				interactable = viewedInteractable;
			} else {
				var myPos         = transform.position;
				var distToOther   = Vector3.Distance( ( ( MonoBehaviour )viewedInteractable ).transform.position, myPos );
				var distToCurrent = Vector3.Distance( ( ( MonoBehaviour )interactable ).transform.position, myPos );

				if ( distToOther < distToCurrent ) {
					interactable = viewedInteractable;
				}
			}
		}

		private void MoveCharacter() {
			// We do not want to keep allocating and discarding the transform. We are controlling something outside the managed space.
			// Also helps keep everything readable.
			var camTransform        = cam.transform;
			var camForwardDirection = camTransform.forward * moveDirection.x;
			var sideDirection       = camTransform.right * moveDirection.z;
			var speedMul            = GetSpeed();
			var myDir               = ( camForwardDirection + sideDirection ).normalized * ( speedMul * Time.deltaTime );

			// Makes sure the player does not go down when looking down.
			// TODO: Disable this if the noclip command is invoked is enabled.
			myDir.y = 0;

			// This is here to check rather the player is grounded or not.
			// We don't want the gravity value to constantly build up, then the player shoots outside
			// the playable area so fast that Unity doesn't like us.
			IsGrounded = Physics.CheckSphere( groundCheck.position, groundDistance, groundMask );

			if ( !IsGrounded ) {
				fallSpeed -= 0.5f * 9.8f * Mathf.Pow( Time.deltaTime, 2 );

				myDir.y = fallSpeed;
			} else if ( IsGrounded && fallSpeed != 0f ) {
				fallSpeed = 0f;
			}

			myDir.y = fallSpeed;

			cc.Move( myDir );
		}

		private float GetSpeed() {
			if ( IsCrouched ) {
				return crouchSpeed;
			}

			// Disabling this for this instance because it makes the code less readable.
			// It also does not make sense to do that in this scenario as it might need to be expanded later.
			// ReSharper disable once ConvertIfStatementToReturnStatement
			if ( IsRunning ) {
				return sprintSpeed;
			}

			return walkSpeed;
		}

		private void MoveCamera() {
			var camTransform = cam.transform;
			var rotateDiff   = Quaternion.Euler( new Vector3( moveCamRot.x, 0f ) * ( camSpeed * Time.deltaTime ) );
			var angle        = Quaternion.Angle( cam.transform.localRotation * rotateDiff, Quaternion.Euler( 0f, camTransform.localRotation.eulerAngles.y, 0f ) );

			if ( angle <= maxLookAngle ) {
				camTransform.Rotate( new Vector3( moveCamRot.x * camSpeed * Time.deltaTime, 0f ) );
			}

			transform.Rotate( new Vector3( 0f, moveCamRot.y * camSpeed * Time.deltaTime, 0f ) );
		}

		private void OnEnable() {
			input?.Game.Enable();
		}

		private void OnDisable() {
			input?.Game.Disable();
		}
	}
}
