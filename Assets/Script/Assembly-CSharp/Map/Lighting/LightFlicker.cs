using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using SCPCB.Remaster.Audio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCPCB.Remaster.Map.Lighting {
	/// <summary>
	/// Lights that are meant to flicker.
	/// </summary>
	[RequireComponent( typeof( Light ) )]
	[RequireComponent( typeof( AudioSource ) )]
	public class LightFlicker : MonoBehaviour {
		/// <summary>
		/// How often the light dims in seconds.
		/// </summary>
		public float FlickRate { get; set; }

		public float RandomAddition { get; set; }

		public AnimationCurve DimCurve { get; set; }

		private float originalIntensity;

		private new Light        light;
		private     AudioSource  source;
		private     AudioManager audioManager;
		private     Coroutine    flickerRoutine;

		private void Start() {
			light        = GetComponent<Light>();
			source       = GetComponent<AudioSource>();
			audioManager = AudioManager.Singleton;

			originalIntensity = light.intensity;

			source.maxDistance  = 10f;
			source.spatialBlend = 1f;
			source.mute         = false;
			source.loop         = false;
		}

		private void OnEnable() {
			flickerRoutine = StartCoroutine( FlickerLightUpdate() );
		}

		private void OnDisable() {
			StopCoroutine( flickerRoutine );
		}

		[SuppressMessage( "ReSharper", "IteratorNeverReturns", Justification = "Doing this since async causes issues." )]
		private IEnumerator FlickerLightUpdate() {
			while ( true ) {
				var additionalSeconds = Random.Range( 0f, RandomAddition );

				yield return new WaitForSeconds( additionalSeconds + FlickRate );

				try {
					source.clip = audioManager["LightFX"]["light1"].Clip;
					source.time = 0f;
				} catch ( Exception e ) {
					Debug.LogException( e, this );
				}

				try {
					source.Play();
				} catch ( Exception e ) {
					Debug.LogException( e, this );
				}

				light.intensity = originalIntensity;

				var time = 0f;

				// I don't know why there isn't a property to get where the last keyframe is.
				while ( time < DimCurve[DimCurve.length - 1].time ) {
					light.intensity = originalIntensity * DimCurve.Evaluate( time );

					time += Time.deltaTime;

					// Wait for next frame.
					yield return null;
				}

				light.intensity = originalIntensity;
			}
		}
	}
}
