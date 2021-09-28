using System;
using System.IO;
using System.Text;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace SCPCB.Remaster {
	/* Here goes my documentation on how the RMesh file is done. Firstly, how I've written things. Anything
	 * that is placed inside square brackets is going to be a type. Anything inside curly brackets means that
	 * it repeats. Inside the type block will usually be a base type and name, or just a type that is explained
	 * later on. Anything beside the curly brackets on the same line at the start means how many times to loop.
	 *
	 * The file starts by a number to indicate how long the string is. The string is NOT a C String at all.
	 * It will not end in a NULL terminator.
	 *
	 * [int headerSize][string header]
	 *
	 * The header should normally be RoomMesh, I have not seen the header start with RoomMesh.HasTriggerBox.
	 * This next section starts with an int, saying how many materials there are. This is then followed by 2
	 * textures, usually a lightmap and the actual texture. This is then followed by vertexes and triangles.
	 * There is, however, an exception in that the textures are not included if it is a collision mesh. Lastly,
	 * if blend is 0, it should be skipped.
	 *
	 * [int matNumbers]{
	 *		[byte blend|no collision]
	 *		[int strSize|no collision]
	 *		[string texFileName|no collision]
	 *		[Vertex]
	 *		[Triangle]
	 * }
	 *
	 * The Vertex section can differ if this is a collision mesh or not. If it is not a collision mesh, then
	 * there are 31 bytes. The first 20 are actually floats that map to X,Y,Z. The Z position, however, does
	 * need to be flipped. After the first 3 floats are the UV floats. The next 8 bytes, I have no clue as to
	 * what they do, but they do not seem needed. Then the remaining 3 bytes are RGB values. If, however, this
	 * mesh is a collision mesh, then there are only 12 bytes that represent floats for X,Y,Z. UV and RGB values
	 * are not encoded.
	 *
	 * [int vertCount]{
	 *		[float x]
	 *		[float y]
	 *		[float z]
	 *		[float u|no collision]
	 *		[float v|no collision]
	 *		[8 unknown bytes|no collision]
	 *		[byte r|no collision]
	 *		[byte g|no collision]
	 *		[byte b|no collision]
	 * }
	 *
	 * After vertex is the triangle section. This section only contains ints, and it contains 3 per triangle.
	 * This starts with the poly count, aka, how many triangles there are encoded. Each triangle goes first,
	 * second, then third.
	 *
	 * [int polyCount]{
	 *		[int first]
	 *		[int second]
	 *		[int third]
	 * }
	 *
	 * After all this, it goes back to the material section. This time it is for the collision mesh. So everything
	 * above that says "no collision" should be ignored on this pass. Once this section is done, there is an EOF
	 * string, but this can be ignored as we already know that this should be the end. So the entire file looks like:
	 *
	 * [int strLen]
	 * [string header]
	 * [int materialCount]{
	 *		x2 {
	 *			[byte blend]
	 *			[int strSize]
	 *			[string texFileName]
	 *		}
	 *		[int vertCount]{
	 *			[float x]
	 *			[float y]
	 *			[float z]
	 *			[float u]
	 *			[float v]
	 *			[8 unknown bytes]
	 *			[byte r]
	 *			[byte g]
	 *			[byte b]
	 *		}
	 *		[int polyCount]{
	 *			[int first]
	 *			[int second]
	 *			[int third]
	 *		}
	 * }
	 * [int materialCount]{
	 *		[int vertCount]{
	 *			[float x]
	 *			[float y]
	 *			[float z]
	 *		}
	 *		[int polyCount]{
	 *			[int first]
	 *			[int second]
	 *			[int third]
	 *		}
	 * } */
	[ScriptedImporter( 1, "rmesh" )]
	public class RMesshImporter : ScriptedImporter {
		private static readonly int    BASE_MAP = Shader.PropertyToID( "_BaseMap" );

		private static Mesh ReadMeshData( BinaryReader binReader, Mesh mesh ) {
			var vertCount = binReader.ReadInt32();
			var verts     = new Vector3[vertCount];
			var uvs       = new Vector2[vertCount];

			for ( var i = 0; i < vertCount; ++i ) {
				var x = binReader.ReadSingle() / 100f;
				var y = binReader.ReadSingle() / 100f;
				var z = binReader.ReadSingle() / 100f;
				var u = -binReader.ReadSingle() + 0.17f;
				var v = -binReader.ReadSingle();

				verts[i] = new Vector3( x, y, z );
				uvs[i]   = new Vector2( u, v );
				
				// I don't know what the next 8 bytes do, but I am also skipping the colors.
				binReader.BaseStream.Position += 11;
			}

			var triCount = binReader.ReadInt32() * 3;
			var tris     = new int[triCount];

			for ( var i = 0; i < triCount; ++i ) {
				tris[i] = binReader.ReadInt32();
			}

			mesh.Clear();

			mesh.vertices  = verts;
			mesh.triangles = tris;
			mesh.uv        = uvs;

			mesh.RecalculateNormals();

			return mesh;
		}

		private string texPath;

		private Shader usedShader;

		private static string GetString( Stream stream ) {
			var len    = stream.ReadByte() | stream.ReadByte() << 8 | stream.ReadByte() << 16 | stream.ReadByte() << 24;
			var buffer = new byte[len];

			stream.Read( buffer, 0, len );

			return Encoding.UTF8.GetString( buffer );
		}

		private static bool IsRMesh( BinaryReader binReader ) {
			var header = GetString( binReader.BaseStream );

			return header == "RoomMesh";
		}

		public override void OnImportAsset( AssetImportContext ctx ) {
			texPath    = Path.Combine( Application.dataPath, "Textures", "Rooms" );
			usedShader = Shader.Find( "Universal Render Pipeline/Lit" );

			ctx.AddObjectToAsset( "commonShader", usedShader );
			
			using var fStream   = new FileStream( ctx.assetPath, FileMode.Open, FileAccess.Read );
			using var binReader = new BinaryReader( fStream, Encoding.UTF8 );

			if ( !IsRMesh( binReader ) ) {
				ctx.LogImportError( "The file is not an RMesh file." );
				return;
			}

			// Setting up the game object before fully using it so it can already be the main object for the asset.
			var gameObj = new GameObject( new FileInfo( ctx.assetPath ).Name );

			// Adding the game object to the asset and making it the main asset.
			ctx.AddObjectToAsset( gameObj.name, gameObj );
			ctx.SetMainObject( gameObj );

			ReadMaterials( binReader, ctx );
		}

		private void ReadMaterials( BinaryReader binReader, AssetImportContext ctx ) {
			const int MAT_NUM = 2;

			var matCount = binReader.ReadInt32();
			var modelObj = new GameObject( "Model" ) {
				transform = {
					parent = ( ( GameObject )ctx.mainObject ).transform
				}
			};
			
			ctx.AddObjectToAsset( modelObj.name, modelObj );

			for ( var i = 0; i < matCount; i++ ) {
				Material mat = default;
				
				for ( var j = 0; j < MAT_NUM; ++j ) {
					if ( binReader.ReadByte() == 0x0 ) {
						continue;
					}

					var texName = GetString( binReader.BaseStream );

					if ( texName.Contains( "lm" ) ) {
						continue;
					}

					mat = new Material( usedShader );
					
					var tex = GetTexture( texName );

					if ( tex == null ) {
						ctx.LogImportWarning( $"Could not find texture `{texName}`. Make sure it is in the `{texPath}` directory." );
					} else {
						mat.SetTexture( BASE_MAP, tex );
						
						ctx.AddObjectToAsset( $"{texName}.{i}", tex );
					}

					ctx.AddObjectToAsset( $"mat.{texName}.{i}", mat );
				}

				// Each "material" will be it's own game object.
				var model = new GameObject( i.ToString() ) {
					transform = {
						parent = modelObj.transform
					}
				};

				var meshFilter     = model.AddComponent<MeshFilter>();
				var renderer = model.AddComponent<MeshRenderer>();
				
				ctx.AddObjectToAsset( $"model.{i}.meshFilter", meshFilter );
				ctx.AddObjectToAsset( $"model.{i}.renderer", renderer );
				ctx.AddObjectToAsset( $"model.{i}", model );

				meshFilter.sharedMesh = ReadMeshData( binReader, new Mesh() );
				
				ctx.AddObjectToAsset( $"model.{i}.mesh", meshFilter.sharedMesh );

				renderer.material = mat;
			}
		}

		private Texture GetTexture( string textureName ) {
			var texPath = Path.Combine( this.texPath, textureName );

			if ( !File.Exists( texPath ) ) {
				return null;
			}

			var myTex = new Texture2D( 1, 1 );

			myTex.LoadImage( File.ReadAllBytes( texPath ) );
			myTex.Apply();

			return myTex;
		}
	}
}
