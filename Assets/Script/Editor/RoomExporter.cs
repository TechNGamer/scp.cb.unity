using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SCPCB.Remaster {
	public class RoomExporter {
		public static readonly  byte[] MAGIC_BYTES         = { 83, 67, 80, 82, 111, 111, 109, 00 };
		private static readonly int    NORMAL_MAP_PROPERTY = Shader.PropertyToID( "_BumpMap" );

		private enum ColliderType : byte {
			Box = 1,
			Mesh,
			Sphere,
			Capsule,
		}

		[MenuItem( "GameObject/SCP: Containment Breach/Export Room" )]
		public static void ExportRoom() {
			var room     = Selection.activeGameObject;
			var filePath = Path.Combine( Application.streamingAssetsPath, "Rooms", $"{room.name}.scp.rm" );

			using var fStream   = new FileStream( filePath, FileMode.Create, FileAccess.Write, FileShare.Read );
			using var binWriter = new BinaryWriter( fStream );

			// This is the magic number, it is 8 bytes long with the last byte being NULL.
			binWriter.Write( MAGIC_BYTES );

			// Start Mesh Data.
			WriteMeshData( binWriter, room.transform.Find( "Model" ) );

			// More code here

			Debug.Log( $"Finished making the SCP: Containment Break Room file. It's located at <b>{filePath}</b>." );
		}

		private static void WriteMeshData( BinaryWriter writer, Transform model ) {
			var childCount = model.childCount;

			writer.Write( childCount );

			for ( var childIndex = 0; childIndex < childCount; ++childIndex ) {
				var  child     = model.GetChild( childIndex );
				var  mesh      = child.GetComponent<MeshFilter>().sharedMesh;
				var  renderer  = child.GetComponent<MeshRenderer>();
				var  colliders = child.GetComponents<Collider>();

				// Begin information placing.
				writer.Write( mesh.vertexCount );
				writer.Write( mesh.triangles.Length / 3 );

				// End information placing. Begin writing data.
				for ( var i = 0; i < mesh.vertexCount; ++i ) {
					var position = mesh.vertices[i];
					var normal   = mesh.normals[i];
					var uv       = mesh.uv[i];

					// Position
					writer.Write( position.x );
					writer.Write( position.y );
					writer.Write( position.z );
					// Normal
					writer.Write( normal.x );
					writer.Write( normal.y );
					writer.Write( normal.z );
					// UV
					writer.Write( uv.x );
					writer.Write( uv.y );
				}

				for ( var i = 0; i < mesh.triangles.Length; ++i ) {
					writer.Write( mesh.triangles[i] );
				}

				// Material Data - URP/Lit only. Models with special shaders need to use Unity's AssetBundle.
				var tex = renderer.material.mainTexture as Texture2D;

				Debug.Assert( tex != null );

				writer.Write( tex.width );
				writer.Write( tex.height );

				for ( var x = 0; x < tex.width; ++x ) {
					for ( var y = 0; y < tex.width; ++y ) {
						var pixel = tex.GetPixel( x, y );

						writer.Write( pixel.r );
						writer.Write( pixel.g );
						writer.Write( pixel.b );
						writer.Write( pixel.a );
					}
				}

				tex = renderer.material.GetTexture( NORMAL_MAP_PROPERTY ) as Texture2D;

				if ( tex != null ) {
					writer.Write( true );
					writer.Write( tex.width );
					writer.Write( tex.height );

					for ( var x = 0; x < tex.width; ++x ) {
						for ( var y = 0; y < tex.height; ++y ) {
							var pixel = tex.GetPixel( x, y );
							
							writer.Write( pixel.r );
							writer.Write( pixel.g );
							writer.Write( pixel.b );
						}
					}
				} else {
					writer.Write( false );
				}

				var colliderData = colliders != null && colliders.Length > 0;
				
				writer.Write( colliderData );

				if ( colliderData ) {
					// We want to know how many colliders there are.
					writer.Write( colliders.Length );
				}

				if ( !colliderData ) {
					return;
				}

				// Writing colliders that are tied to the mesh.
				foreach ( var collider in colliders ) {
					if ( collider is BoxCollider boxCollider ) {
						writer.Write( ( byte )ColliderType.Box );

						var center = boxCollider.center;
						var size   = boxCollider.size;

						// Center
						writer.Write( center.x );
						writer.Write( center.x );
						writer.Write( center.x );
						// Size
						writer.Write( size.x );
						writer.Write( size.x );
						writer.Write( size.x );
					} else if ( collider is CapsuleCollider capCollider ) {
						writer.Write( ( byte )ColliderType.Mesh );

						var center    = capCollider.center;
						var direction = capCollider.direction;
						var radius    = capCollider.radius;
						var height    = capCollider.height;

						// Center
						writer.Write( center.x );
						writer.Write( center.y );
						writer.Write( center.z );
						// Direction
						writer.Write( direction );
						// Radius
						writer.Write( radius );
						// Height
						writer.Write( height );
					} else if ( collider is MeshCollider meshCollider ) {
						writer.Write( ( byte )ColliderType.Capsule );
						writer.Write( meshCollider.convex );
					} else if ( collider is SphereCollider sphereCollider ) {
						writer.Write( ( byte )ColliderType.Sphere );

						var center = sphereCollider.center;
						var radius = sphereCollider.radius;

						// Center
						writer.Write( center.x );
						writer.Write( center.y );
						writer.Write( center.z );
						// Radius
						writer.Write( radius );
					} else {
						continue;
					}

					writer.Write( collider.isTrigger );
				}
			}
		}

		private static void WriteCString( BinaryWriter writer, string str ) {
			var bytes = new byte[str.Length + 1];

			Array.Copy( Encoding.UTF8.GetBytes( str ), bytes, 0 );

			writer.Write( bytes );
		}
	}
}
