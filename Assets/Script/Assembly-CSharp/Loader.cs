using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SCPCB.Remaster {
	public class Loader : MonoBehaviour {
		private void Awake() {
			SceneManager.LoadScene( 1, LoadSceneMode.Single );
		}
	}
}
