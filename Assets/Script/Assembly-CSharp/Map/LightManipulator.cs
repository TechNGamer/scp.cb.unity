using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCPCB.Remaster {
	public class LightManipulator : MonoBehaviour {
		[RequireComponent(typeof(Light))]
		public class LightFlicker : MonoBehaviour {

			/// <summary>
			/// How often the light dims in seconds.
			/// </summary>
			public float FlickRate {
				get;
				set;
			}

			public float RandomAddition {
				get;
				set;
			}

			public AnimationCurve DimCurve {
				get;
				set;
			}

			private Coroutine flickerRoutine;
			private Light     light;

			private void Start() {
				light = GetComponent<Light>();
				
				flickerRoutine = StartCoroutine( Flicker() );
			}

			private void OnEnable() {
				StartCoroutine( Flicker() );
			}

			private void OnDisable() {
				StopCoroutine( flickerRoutine );
			}

			private IEnumerator Flicker() {
				while ( true ) {
					var additionalSeconds = Random.Range( 0f, RandomAddition );
					var time              = 0f;
					var defaultIntens     = light.intensity;
					
					yield return new WaitForSeconds( FlickRate + additionalSeconds );

					while ( time < 1f ) {
						light.intensity = defaultIntens * DimCurve.Evaluate( time );

						time += Time.deltaTime;

						yield return new WaitForEndOfFrame();
					}

					light.intensity = defaultIntens;
				}
			}
		}

		// This hodge bodge of placing fields is weird, but meant to help within the inspector.
		[Header("Light Information")]
		[SerializeField]
		[Tooltip( "The number of lights to randomly disable." )]
		[InspectorName("Maximum Lights On")]
		[Range( 1, 6 )]
		private int maxNumOnLights = 6;

		[SerializeField]
		[Tooltip( "A list of lights in the room." )]
		private GameObject[] lights;

		[Header("Flicker Information")]
		[SerializeField]
		[Tooltip( "Number of lights to flicker." )]
		[Range( 1, 6 )]
		private int numFlickerLights = 1;

		[SerializeField]
		[Tooltip("How long before each dimming, in seconds.")]
		private float flickRate = 2f;

		[SerializeField]
		[Tooltip("How the light is to dim.")]
		private AnimationCurve dimCurve;

		private void Start() {
			TurnRandomLightsOff();
			RandomLightFlicker();
		}

		private void RandomLightFlicker() {
			var flickers = 0;

			while ( flickers != numFlickerLights ) {
				var lightObj = lights[Random.Range( 0, lights.Length )];

				if ( !lightObj.activeInHierarchy ) {
					continue;
				}

				var flicker = lightObj.AddComponent<LightFlicker>();

				flicker.DimCurve  = dimCurve;
				flicker.FlickRate = flickRate;

				++flickers;
			}
		}

		private void TurnRandomLightsOff() {
			var lightsOn = lights.Length;

			while ( lightsOn != maxNumOnLights ) {
				var lightObj = lights[Random.Range( 0, lights.Length )];

				if ( !lightObj.activeInHierarchy ) {
					continue;
				}
				
				lightObj.SetActive( false );

				--lightsOn;
				
				#if UNITY_EDITOR
				Debug.Log( $"Turning of light {lightObj.name}.", lightObj );
				#endif
			}
		}
	}
}
