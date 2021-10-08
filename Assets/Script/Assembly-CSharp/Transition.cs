using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SCPCB.Remaster {
	public class Transition : MonoBehaviour {
		
		// Start is called before the first frame update
		private void Start() {
			StartCoroutine( LoadOtherScene() );
		}

		private IEnumerator LoadOtherScene() {
			var op = SceneManager.LoadSceneAsync( 2, LoadSceneMode.Additive );

			op.allowSceneActivation = false;

			while ( !op.isDone ) {
				Debug.Log( op.progress, this );

				yield return new WaitForEndOfFrame();
			}

			Destroy( this );
		}
	}
}
