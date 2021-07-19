using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


namespace SCPCB.Remaster {
	public class Controller : MonoBehaviour {
		public bool IsGrounded { get; private set; }

		public bool IsCrouched { get; set; }

		public bool IsRunning { get; private set; }

		[SerializeField]
		[Tooltip( "The speed at which the camera moves." )]
		private float camSpeed = 1f;

		[SerializeField]
		[Range( 1f, 5f )]
		[Tooltip( "The speed at which the player walks." )]
		private float walkSpeed = 1f;

		[SerializeField]
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

		private float fallSpeed = 0f;

		private Vector3 moveCamRot;
		private Vector3 moveDirection;

		[SerializeField]
		[Tooltip( "The mask for the ground." )]
		private LayerMask groundMask;

		private Camera              cam;
		private CharacterController cc;
		private MainInput           input;

		[SerializeField]
		[Tooltip( "The object where a ground check occurs." )]
		private Transform groundCheck;

		private void Awake() {
			// This section is mainly used to initialize the events of the new Input System.
			input = new MainInput();

			input.Game.Look.performed += ctx => {
				var value = ctx.ReadValue<Vector2>();
				moveCamRot = new Vector3( -value.y, value.x );
			};
			input.Game.Look.canceled += _ => moveCamRot = Vector3.zero;

			input.Game.Move.performed += ctx => {
				var value = ctx.ReadValue<Vector2>();

				moveDirection = new Vector3( value.y, 0f, value.x );
			};
			input.Game.Move.canceled += _ => moveDirection = Vector3.zero;

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

				IsRunning = true;
			};
			input.Game.Sprint.canceled += _ => { IsRunning = false; };

			input.Game.Enable();
		}

		// Start is called before the first frame update
		private void Start() {
			cam = GetComponentInChildren<Camera>();
			cc  = GetComponent<CharacterController>();

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible   = false;
		}

		// Update is called once per frame
		private void Update() {
			Debug.Log( $"Camera Rotate Direction:\t{moveCamRot}\nPlayer Move Direction:\t{moveDirection}" );

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
			var myDir               = ( camForwardDirection + sideDirection ).normalized * speedMul * Time.deltaTime;

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
			var rotateDiff   = Quaternion.Euler( new Vector3( moveCamRot.x, 0f ) * camSpeed * Time.deltaTime );
			var angle        = Quaternion.Angle( cam.transform.localRotation * rotateDiff, quaternion.Euler( 0f, camTransform.localRotation.eulerAngles.y, 0f ) );

			if ( angle <= 45f ) {
				camTransform.Rotate( new Vector3( moveCamRot.x * camSpeed * Time.deltaTime, 0f ) );
			}

			transform.Rotate( new Vector3( 0f, moveCamRot.y * camSpeed * Time.deltaTime, 0f ) );
		}

		private void OnEnable() {
			input.Game.Enable();
		}

		private void OnDisable() {
			input.Game.Disable();
		}
	}
}
