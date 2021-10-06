using System;
using System.IO;
using System.Text;
using NLayer;
using UnityEngine;

namespace SCPCB.Remaster.Audio {
	/// <summary>
	/// This class represents an audio file in a basic form.
	/// </summary>
	public class NixAudio : Audio {
		private static string GetTempLocation( FileSystemInfo fsInfo ) {
			if ( fsInfo.Name == "StreamingAssets" ) {
				return Path.Combine( Application.temporaryCachePath, "Audio" );
			}

			if ( fsInfo is FileInfo fi ) {
				var fileName = fi.Name.Replace( ".mp3", ".raw" );
				var parent   = fi.Directory;

				return Path.Combine( GetTempLocation( parent ), fileName );
			}

			if ( fsInfo is DirectoryInfo di ) {
				return Path.Combine( GetTempLocation( di.Parent ), di.Name );
			}

			throw new Exception( "How did it get here?" );
		}

		private int sampleLength;
		private int channels;
		private int sampleRate;

		private Stream       stream;
		private BinaryReader binReader;

		/// <summary>
		/// Used to create the audio.
		/// </summary>
		/// <param name="file">The file to load.</param>
		/// <param name="isStreamed">Rather to load it into memory or stream off disk.</param>
		public NixAudio( string file, bool isStreamed ) {
			IsStreamed = isStreamed;

			if ( IsStreamed ) {
				SetupStream( file );
			} else {
				LoadUsingNLayer( file );
			}
		}

		private void SetupStream( string file ) {
			using var readStream = new FileStream( file, FileMode.Open, FileAccess.Read, FileShare.Read );
			using var mpeg       = new MpegFile( readStream );
			var       fInfo      = new FileInfo( file );
			var       newFInfo   = new FileInfo( GetTempLocation( fInfo ) );

			if ( !newFInfo.Directory.Exists ) {
				newFInfo.Directory.Create();
			}

			stream    = new FileStream( newFInfo.FullName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read );
			binReader = new BinaryReader( stream );

			using var binWriter = new BinaryWriter( stream, Encoding.UTF8, true );

			sampleRate   = mpeg.SampleRate;
			channels     = mpeg.Channels;
			sampleLength = ( int )( mpeg.Length / ( channels * sizeof( float ) ) );

			var buffer    = new byte[1024];
			var readCount = 0;

			binWriter.Write( sampleLength );
			binWriter.Write( channels );
			binWriter.Write( sampleRate );

			while ( ( readCount = mpeg.ReadSamples( buffer, 0, 1024 ) ) > 0 ) {
				stream.Write( buffer, 0, readCount );
			}

			Clip = AudioClip.Create(
				string.Empty, // Maybe later to add name, but I don't think it is needed.
				sampleLength,
				channels,
				sampleRate,
				true, // I believe since we are using NLayer and storing it into a byte[], we need to do this.
				ReadData,
				SetPosition
			);
		}

		private void LoadUsingNLayer( string file ) {
			using var fStream = new FileStream( file, FileMode.Open, FileAccess.Read, FileShare.Read );
			using var mpeg    = new MpegFile( fStream );

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
		}

		private unsafe void ReadData( float[] samples ) {
			var raw = new byte[samples.Length * 4];

			stream.Read( raw, 0, raw.Length );

			fixed ( byte* ptrRaw = raw ) {
				var fltArr = ( float* )ptrRaw;

				for ( var i = 0; i < samples.Length; ++i ) {
					samples[i] = fltArr[i];
				}
			}
		}

		private void SetPosition( int pos ) {
			// 12 is the offset from the beginning where the data is stored at.
			stream.Position = pos + 12;
		}

		public override void Dispose() {
			binReader?.Dispose();
			stream?.Dispose();
		}
	}
}
