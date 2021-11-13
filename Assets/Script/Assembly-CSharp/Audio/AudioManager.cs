using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace SCPCB.Remaster.Audio {
	/// <summary>
	/// Manages all of the audio banks that holds all the sounds of the game.
	/// </summary>
	public class AudioManager {
		// Temp class to hold data before getting turned into the actual thing.
		private class JsonAudio {
			public bool   stream;
			public string path;
			public string name;
		}

		// Temp class to hold data before getting turned into the actual thing.
		private class JsonAudioBank {
			public string      name;
			public JsonAudio[] audios;
		}

		public static bool IsInitialized => _singleton != null;

		// Going for a singleton model so that the array access property can be used.
		/// <summary>
		/// Grabs the singleton of this class.
		/// </summary>
		public static AudioManager Singleton => _singleton ??= new AudioManager();

		private static AudioManager _singleton;

		public static void Initialize() {
			if ( _singleton != null ) {
				throw new Exception( "AudioManager has already been initialized." );
			}

			_singleton = new AudioManager();
		}

		/// <summary>
		/// Used to access the audio banks.
		/// </summary>
		/// <param name="key">The value associated with a bank.</param>
		public AudioBank this[ string key ] => audioBanks[key];

		private Dictionary<string, AudioBank> audioBanks;

		protected AudioManager() {
			audioBanks = new Dictionary<string, AudioBank>();

			LoadAudioBanks();

			// Ensures that everything is disposed of.
			Application.quitting += () => {
				foreach ( var pair in audioBanks ) {
					pair.Value.Dispose();
				}

				// Don't have to, but feels right to go ahead and null out the audio bank variable.
				audioBanks = null;
			};
		}

		// Beyond this is a mess of methods to read the JSON without deserializing it.
		#region JSON Reading
		private void LoadAudioBanks() {
			// This section is loading the json file, putting that stream into a reader, then passing that reader into the JsonTextReader to manually deserialize it.
			// Disabling Rider's styling suggestion because it is one declaration without having to add `var` to each stream line.
			// ReSharper disable once JoinDeclarationAndInitializer
			Stream stream;
			#if UNITY_WEBGL || UNITY_ANDROID
			stream = GetJson().GetAwaiter().GetResult();
			#else
			stream = new FileStream( Path.Combine( Application.streamingAssetsPath, "Audio.json" ), FileMode.Open, FileAccess.Read, FileShare.Read );
			#endif
			using var reader     = new StreamReader( stream, Encoding.UTF8 );
			using var jsonReader = new JsonTextReader( reader );

			while ( jsonReader.Read() && jsonReader.TokenType == JsonToken.Comment ) {
				Debug.Log( "Came across another JSON comment." );
			}

			if ( jsonReader.TokenType != JsonToken.StartArray ) {
				throw new ArgumentException( $"Expected an array, but got `{Enum.GetName( typeof( JsonToken ), jsonReader.TokenType )}" );
			}

			foreach ( var jsonBank in ReadJsonBank( jsonReader ) ) {
				var audios = new AudioCore[jsonBank.audios.Length];
				var names  = new string[jsonBank.audios.Length];

				for ( var i = 0; i < audios.Length; ++i ) {
					var jsonAudio = jsonBank.audios[i];

					audios[i] = GetAudioObject( jsonAudio.path, jsonAudio.stream );
					names[i]  = jsonAudio.name;
				}

				var bank = new AudioBank( audios.ToArray(), names.ToArray() );

				audioBanks.Add( jsonBank.name, bank );
			}
		}

		#if UNITY_WEBGL || UNITY_ANDROID
		private static async Task<Stream> GetJson() {
			var       streamingUri = new Uri( Application.streamingAssetsPath );
			using var webHelper    = UnityWebRequest.Get( new Uri( streamingUri, "Audio.json" ) );
			var       request      = webHelper.SendWebRequest();

			while ( !request.isDone ) {
				await Task.Delay( 256 );
			}

			return new MemoryStream( webHelper.downloadHandler.data );
		}
		#endif

		// This method knows which Audio object to hand off because of the pre-processors.
		private static AudioCore GetAudioObject( string path, bool stream ) {
			#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			return new WindowsAudio( Path.Combine( Application.streamingAssetsPath, path ), stream );
			#elif (UNITY_WEBGL || UNITY_ANDROID) && !UNITY_EDITOR
			// Because this is going over the web, it is probably best to just download everything instead of streaming.
			return new WebAudio( path );
			#else
			return new NixAudio( Path.Combine( Application.streamingAssetsPath, path ), stream );
			#endif
		}

		private static IEnumerable<JsonAudioBank> ReadJsonBank( JsonReader reader ) {
			var audios = new List<JsonAudio>();
			var name   = string.Empty;

			while ( reader.Read() ) {
				if ( reader.TokenType == JsonToken.EndArray ) {
					break;
				}

				if ( reader.TokenType == JsonToken.EndObject ) {
					yield return new JsonAudioBank() {
						name   = name,
						audios = audios.ToArray()
					};

					audios.Clear();

					continue;
				}

				if ( reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Comment ) {
					continue;
				}

				if ( reader.TokenType != JsonToken.PropertyName ) {
					throw new ArgumentException( $"Expected property, but got {Enum.GetName( typeof( JsonToken ), reader.TokenType )}" );
				}

				if ( ( string )reader.Value == "audio" ) {
					audios.AddRange( ReadAudioArray( reader ) );
				} else if ( ( string )reader.Value == "name" ) {
					name = reader.ReadAsString();
				} else {
					throw new ArgumentException( $"Unexpected token type: {Enum.GetName( typeof( JsonToken ), reader.TokenType )}" );
				}
			}
		}

		private static IEnumerable<JsonAudio> ReadAudioArray( JsonReader reader ) {
			string name   = string.Empty, path = string.Empty;
			var    stream = false;

			while ( reader.Read() ) {
				if ( reader.TokenType == JsonToken.EndObject ) {
					yield return new JsonAudio {
						name   = name,
						path   = path,
						stream = stream
					};
				} else if ( reader.TokenType == JsonToken.PropertyName ) {
					if ( reader.Value == null ) {
						throw new ArgumentNullException( nameof( reader.Value ), "Expected a value, but got null instead." );
					}

					if ( ( string )reader.Value == "name" ) {
						name = reader.ReadAsString();
					} else if ( ( string )reader.Value == "stream" ) {
						// ReSharper disable once PossibleInvalidOperationException
						stream = ( bool )reader.ReadAsBoolean();
					} else if ( ( string )reader.Value == "path" ) {
						path = reader.ReadAsString();
					} else {
						throw new Exception( $"Unknown property: {reader.Value}" );
					}
				} else if ( reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.StartArray || reader.TokenType == JsonToken.Comment ) {
					// This is not redundant Rider, it is here to avoid the throw statement.
					// ReSharper disable once RedundantJumpStatement
					continue;
				} else if ( reader.TokenType == JsonToken.EndArray ) {
					break;
				} else {
					throw new Exception( $"Unexpected Token Type: {Enum.GetName( typeof( JsonToken ), reader.TokenType )}" );
				}
			}
		}
		#endregion
	}
}
