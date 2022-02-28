using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCPCB.Remaster.UI {
	[RequireComponent( typeof( TMP_Text ) )]
	public class GhostText : MonoBehaviour {
		[SerializeField]
		private string[] phrases;

		private TMP_Text  text;
		private Coroutine ghostText;

		// Start is called before the first frame update
		private void Start() {
			text = GetComponent<TMP_Text>();
		}

		private void Update() {
			if ( ghostText != null ) {
				return;
			}

			ghostText = StartCoroutine( DisplayText() );
		}

		private IEnumerator DisplayText() {
			var randNumber = Random.Range( 0, phrases.Length );

			text.text = phrases[randNumber];
			text.ForceMeshUpdate();

			var textSize    = text.textBounds.size;
			var myTransform = ( RectTransform )transform;
			var posX        = Random.value * ( 1920f - textSize.x * 1.5f );
			var posY        = Random.value * ( 1080f - textSize.y * 1.5f );

			myTransform.anchoredPosition = new Vector2( posX, posY );

			yield return new WaitForSeconds( 0.75f );

			text.text = string.Empty;

			yield return new WaitForSeconds( Random.value * 30f );

			ghostText = null;
		}
	}
}
