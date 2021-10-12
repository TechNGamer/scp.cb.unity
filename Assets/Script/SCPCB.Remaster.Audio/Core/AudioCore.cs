using System;
using System.IO;
using UnityEngine;

namespace SCPCB.Remaster.Audio {
	/// <summary>
	/// This class represents an audio file in a basic form.
	/// </summary>
	public abstract class AudioCore : IDisposable {
		/// <summary>
		/// Informs rather or not the audio file is being streamed.
		/// </summary>
		// ReSharper disable once MemberCanBePrivate.Global
		public bool IsStreamed { get; protected set; }

		/// <summary>
		/// The audio clip that is used to play through Unity.
		/// </summary>
		public AudioClip Clip { get; protected set; }

		protected Stream stream;

		public virtual void Dispose() {
			stream?.Dispose();
		}
	}
}
