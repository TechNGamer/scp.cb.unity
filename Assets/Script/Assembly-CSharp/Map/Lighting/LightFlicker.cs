using System;
using System.Threading.Tasks;
using SCPCB.Remaster.Audio;
using UnityEngine;
using Random = UnityEngine.Random;
using ULight = UnityEngine.Light;

namespace SCPCB.Remaster.Map.Lighting {
	/// <summary>
	/// Lights that are meant to flicker.
	/// </summary>
	[RequireComponent( typeof( ULight ) )]
	[RequireComponent( typeof( AudioSource ) )]
	public class LightFlicker : MonoBehaviour {
		/// <summary>
		/// How often the light dims in seconds.
		/// </summary>
		public int FlickRate { get; set; }

		public float RandomAddition { get; set; }

		public AnimationCurve DimCurve { get; set; }

		private float originalIntensity;

		private new ULight       light;
		private     AudioSource  source;
		private     AudioManager audioManager;

		private void Start() {
			light  = GetComponent<ULight>();
			source = GetComponent<AudioSource>();

			source.maxDistance  = 10f;
			source.spatialBlend = 1f;
			source.mute         = false;
			source.loop         = false;
		}

		// Totally forgot I can make Update be async, makes it easier to halt it until it needs to resume again.
		private async void Update() {
			var additionalSeconds = ( int )( Random.Range( 0f, RandomAddition ) * 1000 );

			await Task.Delay( additionalSeconds + FlickRate );

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

			await Task.Run( () => {
				var time = 0f;

				while ( time < 1f ) {
					light.intensity = originalIntensity * DimCurve.Evaluate( time );

					time += Time.deltaTime;
				}
			} );

			light.intensity = originalIntensity;
		}
	}
}
