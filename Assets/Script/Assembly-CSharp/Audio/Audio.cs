/* As a heads up, this file is a bit messy, and there is a reason for it.
 * The file is messy because I am using NAudio for the Windows build, but
 * also using NLayer for the non-Windows builds. This makes it a bit messy
 * to work with since the file needs to change to reflect which build it is
 * going for or editor that is being used.
 *
 * The 3 lines below help cut down on the crud by making it it known if Windows
 * is in use without having to type `UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN`
 * on everything that requires it. By the way, I do hate using Preprocessors
 * as they do ruin the flow of code. But they are needed here. */

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#define WINDOWS
#endif

using System;
using System.IO;
using System.Reflection;
#if WINDOWS
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
#else
using NLayer;
#endif
using UnityEngine;

namespace SCPCB.Remaster.Audio {
	/// <summary>
	/// This class represents an audio file in a basic form.
	/// </summary>
	public class Audio : IDisposable {
		/// <summary>
		/// Informs rather or not the audio file is being streamed.
		/// </summary>
		public bool IsStreamed { get; }

		/// <summary>
		/// The audio clip that is used to play through Unity.
		/// </summary>
		public AudioClip Clip { get; private set; }

		#if WINDOWS
		private Mp3FileReader mp3FileReader;
		private SampleChannel sampleChannel;
		#else
		private MpegFile      mpeg;
		#endif
		private Stream stream;

		/// <summary>
		/// Used to create the audio.
		/// </summary>
		/// <param name="file">The file to load.</param>
		/// <param name="isStreamed">Rather to load it into memory or stream off disk.</param>
		public Audio( string file, bool isStreamed ) {
			IsStreamed = isStreamed;

			#if WINDOWS
			LoadUsingNAudio( file );
			#else
			LoadUsingNLayer(file);
			#endif
		}

		#if WINDOWS
		private void LoadUsingNAudio( string file ) {
			stream        = new FileStream( file, FileMode.Open, FileAccess.Read, FileShare.Read );
			mp3FileReader = new Mp3FileReader( stream );
			sampleChannel = new SampleChannel( mp3FileReader, false );

			Clip = AudioClip.Create(
				string.Empty,
				GetNumberOfSamples( mp3FileReader ),
				mp3FileReader.Mp3WaveFormat.Channels,
				mp3FileReader.Mp3WaveFormat.SampleRate,
				true,
				ReadData,
				SetPosition
			);
		}

		// This method here is to grab the total samples field from NAudio, because who needs that exposed?
		private static int GetNumberOfSamples( Mp3FileReader reader ) {
			var type      = reader.GetType().BaseType;
			var fieldInfo = type.GetField( "totalSamples", BindingFlags.NonPublic | BindingFlags.Instance);
			var value     = fieldInfo.GetValue( reader );

			return value is long valL ? (int)valL : -1;
		}
		#else
		private void LoadUsingNLayer(string file) {
			if ( IsStreamed ) {
				var fStream = new FileStream( file, FileMode.Open, FileAccess.Read, FileShare.Read );

				mpeg = new MpegFile( fStream );

				Clip = AudioClip.Create(
					string.Empty, // Maybe later to add name, but I don't think it is needed.
					( int )( mpeg.Length / ( mpeg.Channels * sizeof( float ) ) ), // Don't remember how this works out.
					mpeg.Channels,
					mpeg.SampleRate,
					true, // I believe since we are using NLayer and storing it into a byte[], we need to do this.
					ReadData,
					SetPosition
				);
			} else {
				using var fStream = new FileStream( file, FileMode.Open, FileAccess.Read, FileShare.Read );

				mpeg = new MpegFile( fStream );

				var samples = new float[mpeg.Length];

				mpeg.ReadSamples( samples, 0, samples.Length );

				Clip = AudioClip.Create(
					string.Empty,
					( int )( mpeg.Length / ( mpeg.Channels * sizeof( float ) ) ),
					mpeg.Channels,
					mpeg.SampleRate,
					false
				);

				Clip.SetData( samples, 0 );

				mpeg.Dispose();
				// Making sure the field cannot be called again.
				mpeg = null;
			}
		}
		#endif

		private void ReadData( float[] samples ) {
			#if WINDOWS
			sampleChannel.Read( samples, 0, samples.Length );
			#else
			mpeg.ReadSamples( samples, 0, samples.Length );
			#endif
		}

		private void SetPosition( int pos ) {
			#if WINDOWS
			mp3FileReader.Position = pos;
			#else
			mpeg.Position = pos;
			#endif
		}

		public void Dispose() {
			#if WINDOWS
			mp3FileReader?.Dispose();
			#else
			mpeg?.Dispose();
			#endif

			stream?.Dispose();
		}
	}
}
