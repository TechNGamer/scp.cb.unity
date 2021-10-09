using System.Collections;
using SCPCB.Remaster.Audio;
using UnityEngine;

namespace SCPCB.Remaster.Player {
	/// <summary>
	/// The player controller.
	/// </summary>
	[RequireComponent( typeof( AudioSource ) )]
	public class PlayerController : MonoBehaviour {
		private static PlayerController pPlayerController;

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

		private void Awake() {
			// Want to make sure that there is only one player controller.
			if ( pPlayerController ) {
				Destroy( this );
				return;
			}

			pPlayerController = this;
			audioManager = AudioManager.Singleton;

			aSource = GetComponent<AudioSource>();

			aSource.loop = false;

			StartCoroutine( PlaySteps() );

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

				Debug.Log( "Move Preformed has been called.", this );
			};
			input.Game.Move.canceled += _ => {
				moveDirection = Vector3.zero;

				Debug.Log( "Move Canceled has been called.", this );
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

				Debug.Log( "Player is running." );

				IsRunning = true;
			};
			input.Game.Sprint.performed += _ => { IsRunning = moveDirection != Vector3.zero; };
			input.Game.Sprint.canceled += _ => {
				Debug.Log( "Player is no longer running." );

				IsRunning = false;
			};

			input.Game.Enable();
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

		private IEnumerator PlaySteps() {
			while ( true ) {
				yield return new WaitUntil( () => moveDirection != Vector3.zero && !aSource.isPlaying );

				aSource.clip = IsRunning ? audioManager["Steps"]["Run1"].Clip : audioManager["Steps"]["Step1"].Clip;

				aSource.Play();
			}
		}

		// Update is called once per frame
		private void Update() {
			MoveCharacter();
			MoveCamera();
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
