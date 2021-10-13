using System;
using System.Collections;
using System.Threading.Tasks;
using SCPCB.Remaster.Audio;
using UnityEngine;
// Want to use Unity's Random instead of System.Random.
using Random = UnityEngine.Random;
// Because the namespace includes Light, we need to alias ULight to Unity's Light class.
using ULight = UnityEngine.Light;

namespace SCPCB.Remaster.Map.Lighting {
	public abstract class LightManipulator : MonoBehaviour {
		protected enum LightSettings : byte {
			On      = 0,
			Off     = 1,
			Flicker = 2
		}

		// This hodge bodge of placing fields is weird, but meant to help within the inspector.
		[Header( "Light Information")]
		[SerializeField]
		[Tooltip( "The number of lights to randomly disable." )]
		[Range( 1, 6 )]
		protected int maxNumOnLights = 6;

		[Header( "Flicker Information")]
		[SerializeField]
		[Tooltip( "Number of lights to flicker." )]
		[Range( 1, 6 )]
		protected int numFlickerLights = 1;

		[SerializeField]
		[Tooltip( "How long before each dimming, in seconds." )]
		protected float flickRate = 2f;

		[SerializeField]
		[Tooltip( "A range between 0 and 5f." )]
		[Range( 0f, 5f )]
		protected float randomInterval = 1f;

		[SerializeField]
		[Tooltip( "How the light is to dim." )]
		protected AnimationCurve dimCurve;
	}
}
