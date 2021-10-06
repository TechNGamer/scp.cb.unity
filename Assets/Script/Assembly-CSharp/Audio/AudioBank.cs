using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPCB.Remaster.Audio {
	/// <summary>
	/// Keeps track of all the audio objects assigned to it.
	/// </summary>
	public class AudioBank {

		public Audio this[ string key ] => bank[key];

		public string[] GetKeys => bank.Keys.ToArray();

		private Dictionary<string, Audio> bank;

		// This is internal so as not to allow other to create them willy nilly.
		internal AudioBank( Audio[] audioList, string[] audioNames ) {
			if ( audioList.Length != audioNames.Length ) {
				throw new ArgumentOutOfRangeException($"{nameof(audioList)} and {nameof(audioNames)} must be the same length");
			}

			bank = new Dictionary<string, Audio>();
			
			for ( var i = 0; i < audioList.Length; ++i ) {
				var audio = audioList[i];
				var name  = audioNames[i];
				
				bank.Add( name, audio );
			}
		}

		/* This is to help with cleaning up the disposables.
		 * Each AudioBank is in charge of disposing of the Audio objects.
		 * This way, AudioManager doesn't have to call Dispose on them all.
		 * To help reduce clutter in it's delegate, and because it makes
		 * more sense to have the bank dispose of them as they keep track
		 * of them. And AudioManager keeps track of the banks. */
		internal void Dispose() {
			foreach ( var pair in bank ) {
				pair.Value.Dispose();
			}

			// Feels right to null out the bank variable.
			bank = null;
		}

	}
}
