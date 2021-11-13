using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace SCPCB.Remaster.Map {
	public static class RoomManager {

		private class JsonRoom {
			[JsonProperty("name")]
			public string Name { get; set; }

			[JsonProperty("chance")]
			public double Chance { get; set; }

			[JsonProperty("AssetName")]
			public string AssetName { get; set; }
		}

		private static LinkedList<Room> rooms;

	}
}
