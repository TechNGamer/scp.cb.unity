using UnityEngine;

namespace SCPCB.Remaster.Map {
	public class Room {

		public float SpawnChance {
			get;
			private set;
		}

		public string Name {
			get;
			private set;
		}

		public GameObject GameRoom {
			get;
			private set;
		}

		internal Room( float chance, string name, string assetName ) {
			
		}
	}
}
