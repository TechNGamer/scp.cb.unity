using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCPCB.Remaster.Door {
	public abstract class Button : MonoBehaviour {

		public event Action ButtonPressed; 

		protected void Awake() {
			var door = GetComponentInParent<Door>();
		}
	}
}
