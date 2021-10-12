using System;
using System.IO;
using System.Reflection;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using UnityEngine;

namespace SCPCB.Remaster.Audio {
	public class WindowsAudio : AudioCore {
		
		private Mp3FileReader mp3FileReader;
		private SampleChannel sampleChannel;

		public WindowsAudio( string file, bool isStreamed ) {
			IsStreamed = isStreamed;

			if ( IsStreamed ) {
				
			} else {
				LoadUsingNAudio( file );
			}
		}
		
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

		private void ReadData( float[] samples ) {
			sampleChannel.Read( samples, 0, samples.Length );
		}

		private void SetPosition( int pos ) {
			mp3FileReader.Position = pos;
		}

		public override void Dispose() {
			mp3FileReader?.Dispose();
			stream?.Dispose();
		}
		
	}
}
