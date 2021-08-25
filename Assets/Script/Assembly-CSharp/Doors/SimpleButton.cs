using UnityEngine;

namespace SCPCB.Remaster.Door {
	public class SimpleButton : Button {
		protected void Awake() {
			base.Awake();
			
			Debug.Log( $"`{GetType().FullName}` awoke" );
		}
	}
}
