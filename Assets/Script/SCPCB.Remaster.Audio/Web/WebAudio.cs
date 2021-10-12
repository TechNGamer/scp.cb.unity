using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SCPCB.Remaster.Audio {
	public class WebAudio : AudioCore {
		public static readonly Uri STREAMING_ASSETS = new Uri( Application.streamingAssetsPath );
		
		public WebAudio( string subPath ) {
			if ( subPath.StartsWith( "/" ) ) {
				subPath = subPath.Remove( 0, 1 );
			}

			var webRequest = UnityWebRequestMultimedia.GetAudioClip( new Uri( STREAMING_ASSETS, subPath ), AudioType.MPEG );
			var request    = webRequest.SendWebRequest();

			while ( !request.isDone ) {
				Task.Delay( 256 ).GetAwaiter().GetResult();
			}

			Clip = ( ( DownloadHandlerAudioClip )webRequest.downloadHandler ).audioClip;

			IsStreamed = false;
		}
	}
}
