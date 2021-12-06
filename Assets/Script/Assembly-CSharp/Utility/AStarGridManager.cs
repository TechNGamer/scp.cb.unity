using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SCPCB.Remaster.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCPCB.Remaster.Utility {
	/// <summary>
	/// This class helps compute a path using A*, and must be part of a game object.
	/// </summary>
	/// <remarks>
	/// This class does not keep track of any other A* managers, that is because this class is not meant as a single A* manager.
	/// There can be multiple A* managers, thus, in order to use one, you must have a reference to it.
	/// </remarks>
	public sealed class AStarGridManager : MonoBehaviour {
		// Since these can add a lot of lines, and it looks ugly to be included, I'm putting them in the region pre-processor.
		#region Nested Types
		/// <summary>
		/// Represents a node on the grid system.
		/// </summary>
		[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
		public class Node : IEquatable<Node> {
			public static bool operator ==( Node l, Node r ) => !ReferenceEquals( l, null ) && l.Equals( r );

			public static bool operator !=( Node l, Node r ) => !( l == r );

			// This is meant as a shortcut, and to make sure that the world position does not change.

			/// <summary>
			/// The X position it is located at in the world.
			/// </summary>
			public float WorldX => worldPosition.x;

			/// <summary>
			/// The Y location of where it is at in the world.
			/// </summary>
			public float WorldY => worldPosition.y;

			/// <summary>
			/// The Z location of where it is at in the world.
			/// </summary>
			public float WorldZ => worldPosition.z;

			/// <summary>
			/// If this node is walkable or not.
			/// </summary>
			public bool IsWalkable { get; }

			public bool IsGround { get; }

			internal readonly Vector3    worldPosition;
			internal readonly Vector3Int gridPos;

			internal Node( Vector3 worldPos, Vector3Int gridPos, bool isWalkable, bool isGround = true ) {
				IsWalkable = isWalkable;
				IsGround   = isGround;

				worldPosition = worldPos;
				this.gridPos  = gridPos;
			}

			public bool Equals( Node other ) {
				if ( ReferenceEquals( null, other ) ) {
					return false;
				}

				if ( ReferenceEquals( this, other ) ) {
					return true;
				}

				return GetHashCode() == other.GetHashCode();
			}

			public override bool Equals( object obj ) {
				if ( ReferenceEquals( null, obj ) ) {
					return false;
				}

				if ( ReferenceEquals( this, obj ) ) {
					return true;
				}

				return obj.GetType() == GetType() && Equals( ( Node )obj );
			}

			public override int GetHashCode() {
				unchecked {
					var hashCode = IsWalkable.GetHashCode();
					hashCode = ( hashCode * 397 ) ^ worldPosition.GetHashCode();
					hashCode = ( hashCode * 397 ) ^ gridPos.GetHashCode();
					return hashCode;
				}
			}
		}

		/// <summary>
		/// Represents a node on a path.
		/// </summary>
		/// <remarks>
		/// This class is meant as a thread-safe way to calculate a path without modifying the actual <see cref="AStarGridManager.Node"/>.
		/// </remarks>
		[SuppressMessage( "ReSharper", "NonReadonlyMemberInGetHashCode" )]
		[SuppressMessage( "ReSharper", "MemberCanBePrivate.Local" )]
		private class NodePath : IEquatable<Node>, IEquatable<NodePath>, Heap<NodePath>.IHeapItem<NodePath> {
			// Overriding the equality operator means that it can forward the check to the node.
			public static bool operator ==( NodePath l, NodePath r ) {
				var leftNull  = ReferenceEquals( l, null );
				var rightNull = ReferenceEquals( r, null );

				if ( leftNull && rightNull ) {
					return true;
				}

				if ( leftNull ^ rightNull ) {
					return false;
				}

				Debug.Assert( l != null );
				Debug.Assert( r != null );

				return l.Equals( r );
			}

			public static bool operator !=( NodePath l, NodePath r ) => !( l == r );

			public static implicit operator Node( NodePath n ) => n.node;

			// Object
			public int HeapIndex { get; set; }

			// Is meant as a quick access.
			public bool IsWalkable => node.IsWalkable;

			public int FCost => gCost + hCost;

			public Vector3 WorldPosition => node.worldPosition;

			public int gCost;
			public int hCost;

			public readonly Node     node;
			public          NodePath parent;

			// The constructor helps ensures that a node was passed to it.
			public NodePath( Node node ) {
				if ( node == null ) {
					throw new ArgumentNullException(
						nameof( node ),
						"A node must be provided in order to build a proper path."
					);
				}

				this.node = node;
			}

			// This method will throw an exception if the node is null, since this object's hash is the same as the nodes.
			public override int GetHashCode() {
				if ( node == null ) {
					throw new NullReferenceException( "Node must be non-null to generate hash." );
				}

				return node.GetHashCode();
			}

			public bool Equals( Node other ) => Equals( ( object )other );

			public bool Equals( NodePath other ) => Equals( ( object )other );

			public int CompareTo( NodePath other ) {
				var compareNum = FCost.CompareTo( other.FCost );

				return compareNum == 0 ? hCost.CompareTo( other.hCost ) : -compareNum;
			}

			public override bool Equals( object obj ) =>
				obj switch {
					NodePath np => node == np.node,
					Node n      => node == n,
					_           => ReferenceEquals( this, obj )
				};

			public override string ToString() => WorldPosition.ToString();
		}
		#endregion

		// Everything that is static of this class belongs here.
		#region Static
		/* As the method name says, it get's the distance between 2 nodes.
		 * However, it also adds some weights to the values so they are not small. */
		private static int GetDistance( Node nodeA, Node nodeB ) {
			var distX = Mathf.Abs( nodeA.gridPos.x - nodeB.gridPos.x );
			var distY = Mathf.Abs( nodeA.gridPos.y - nodeB.gridPos.y );
			int bigger;
			int smaller;

			if ( distX > distY ) {
				bigger  = distX;
				smaller = distY;
			} else {
				bigger  = distY;
				smaller = distX;
			}

			return 14 * smaller + 10 * ( bigger - smaller );
		}
		#endregion

		/// <summary>
		/// How big the nodes are from their center out in a single direction.
		/// </summary>
		/// <remarks>
		/// Even though it says it is a diameter, the nodes are square.
		/// This property is meant as quick way to get the half measure.
		/// </remarks>
		// ReSharper disable once MemberCanBePrivate.Global
		public float NodeDiameter => nodeSize / 2f;

		public bool IsGridReady { get; private set; }

		[SerializeField]
		[Tooltip( "How big each node is." )]
		private float nodeSize;

		#if UNITY_EDITOR
		[SerializeField]
		[Tooltip( "Shows the grid within the editor. (Field won't be in finial build.)" )]
		private bool showGrid;
		#endif

		[SerializeField]
		[Tooltip( "The meshes on the grid that cannot be walked through." )]
		private LayerMask blockMask;

		[SerializeField]
		[Tooltip( "The meshes that are part of the ground." )]
		private LayerMask groundMask;

		[SerializeField]
		[FormerlySerializedAs( "gridSize" )]
		[Tooltip( "The size of the grid to make." )]
		private Vector3 gridWorldSize;

		private Vector3Int gridSize;
		private Vector3    position;

		private Node[,,] grid;

		#region Unity Events
		private void Awake() {
			var gridSizeX = Mathf.RoundToInt( gridWorldSize.x / nodeSize );
			var gridSizeY = Mathf.RoundToInt( gridWorldSize.y / nodeSize );
			var gridSizeZ = Mathf.RoundToInt( gridWorldSize.z / nodeSize );

			gridSize = new Vector3Int( gridSizeX, gridSizeY, gridSizeZ );

			// Since the grid should never move from where it is placed, we can do this and now that worldPos is accessible on any thread.
			position = transform.position;

			StartCoroutine( CreateGrid( gridSizeX, gridSizeY, gridSizeZ ) );
		}

		private void Reset() => Awake();

		[SuppressMessage( "ReSharper", "LocalVariableHidesMember" )]
		private void OnDrawGizmosSelected() {
			if ( gridWorldSize == Vector3.zero ) {
				return;
			}

			var position = transform.position;

			Gizmos.DrawWireCube( position, gridWorldSize );

			// Bottom Left
			Gizmos.color = Color.black;

			var bottomLeft = position - gridWorldSize / 2f;
			Gizmos.DrawWireSphere( bottomLeft, 1f );

			if ( grid == null || !showGrid ) {
				return;
			}

			for ( var x = 0; x < grid.GetLength( 0 ); ++x ) {
				for ( var y = 0; y < grid.GetLength( 1 ); ++y ) {
					for ( var z = 0; z < gridSize.z; ++z ) {
						Gizmos.color = grid[x, y, z].IsGround ? Color.yellow : Color.white;
						Gizmos.color = grid[x, y, z].IsWalkable ? Gizmos.color : Color.red;

						Gizmos.DrawCube( grid[x, y, z].worldPosition, new Vector3( nodeSize, nodeSize, nodeSize ) * 0.25f );
					}
				}
			}
		}
		#endregion

		// ReSharper disable once MemberCanBePrivate.Global
		public Node GetNodeFromWorld( Vector3 worldPos ) {
			// Makes an obsolete position a relative one. Which makes it good to use anywhere.
			worldPos -= position;

			/* Getting the logical position of it's location on the grid.
			 * It does this by adding the world position to half of the grid world size.
			 * It then divides that by the grid world size to get where it is between 0 and 1. */
			var perX = Mathf.Clamp01( ( worldPos.x + gridWorldSize.x / 2 ) / gridWorldSize.x );
			var perY = Mathf.Clamp01( ( worldPos.y + gridWorldSize.y / 2 ) / gridWorldSize.y );
			var perZ = Mathf.Clamp01( ( worldPos.z + gridWorldSize.z / 2 ) / gridWorldSize.z );

			/* Get's the XY coords by multiplying the grid size by the percentage, then subtracting 1.
			 * The reason for not using GridWorldSize for this is that it needs to know how big the grid
			 * it, so it can get the general location. */
			var x = Mathf.RoundToInt( perX * ( gridSize.x - 1 ) );
			var y = Mathf.RoundToInt( perY * ( gridSize.y - 1 ) );
			var z = Mathf.RoundToInt( perZ * ( gridSize.z - 1 ) );

			// Returns that node at that position.
			return grid[x, y, z];
		}

		/// <summary>
		/// Same as <see cref="FindPath"/>, but does it on another Task.
		/// </summary>
		/// <param name="start">The starting world position to calculate from.</param>
		/// <param name="end">The end world position.</param>
		/// <param name="token">The token to cancel the job.</param>
		/// <returns>An array of nodes to follow.</returns>
		public Task<Node[]> FindPathAsync( Vector3 start, Vector3 end, CancellationToken token ) => Task.Run( () => {
			try {
				return FindPath( start, end );
			} catch ( Exception e ) {
				Debug.LogException( e );
				throw;
			}
		}, token );

		/// <summary>
		/// Calculates the path from the <paramref name="start"/> to the <paramref name="end"/>.
		/// </summary>
		/// <remarks>
		/// This method is blocking, so it is recommended to use <see cref="FindPathAsync"/> as it does the calculations on a different <see cref="Task{TResult}"/>.
		/// </remarks>
		/// <seealso cref="FindPathAsync"/>
		/// <param name="start">The starting world position.</param>
		/// <param name="end">The target world position.</param>
		/// <returns>An array of nodes of the path.</returns>
		/// <exception cref="Exception">You should never see this. If you do, something went horrible wrong.</exception>
		[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
		public Node[] FindPath( Vector3 start, Vector3 end ) {
			if ( !IsGridReady ) {
				return null;
			}

			var startNode  = new NodePath( GetNodeFromWorld( start ) );
			var targetNode = new NodePath( GetNodeFromWorld( end ) );
			var openSet    = new Heap<NodePath>( grid.Length );
			var closedSet  = new HashSet<NodePath>();

			openSet.Add( startNode );

			while ( openSet.Count > 0 ) {
				var currentNode = openSet.RemoveFirst();
				closedSet.Add( currentNode );

				// This is the first time where a LinkedList actually comes in handy.
				// This method basically created a LinkedList, but in one direction. Using C#'s built in LinkedList means that we can `AddFirst` the parents.
				// Which means that we are reversing the list while also adding it to another. Then we use Linq to return an array.
				if ( currentNode == targetNode ) {
					var list = new LinkedList<Node>();
					var node = currentNode;

					do {
						list.AddFirst( node.node );

						node = node.parent;
					} while ( node != null );

					return list.ToArray();
				}

				foreach ( var neighborNode in GetNeighborNodes( currentNode ) ) {
					if ( !neighborNode.IsWalkable || closedSet.Contains( neighborNode ) ) {
						continue;
					}

					var newMoveCost = currentNode.gCost + GetDistance( currentNode, neighborNode );

					if ( newMoveCost > neighborNode.gCost && openSet.Contains( neighborNode ) ) {
						continue;
					}

					neighborNode.gCost  = newMoveCost;
					neighborNode.hCost  = GetDistance( neighborNode, targetNode );
					neighborNode.parent = currentNode;

					if ( !openSet.Contains( neighborNode ) ) {
						openSet.Add( neighborNode );
					}
				}
			}

			return null;
		}

		private IEnumerator CreateGrid( int sizeX, int sizeY, int sizeZ ) {
			const float waitFramerate = 1f / 24f;
			var         bottomLeft    = transform.position - gridWorldSize / 2f;
			var         boxCheck      = new Vector3( nodeSize, nodeSize, nodeSize ) / 2f;

			yield return new WaitForEndOfFrame();

			// Since the path finding is done via A*, the best way to calculate the path is to make a grid.
			// There is probably a better way to do this but for now the grid will have to do.
			grid = new Node[sizeX, sizeY, sizeZ];

			// This is the part that is of course going to take the longest. Should probably put it on a different task.
			for ( var x = 0; x < sizeX; ++x ) {
				for ( var y = 0; y < sizeY; ++y ) {
					for ( var z = 0; z < sizeZ; ++z ) {
						var worldPoint = bottomLeft + new Vector3(
							x * nodeSize + NodeDiameter, // Each location has to be offset the actual size
							y * nodeSize + NodeDiameter, // of the node, plus the node's diameter to get it
							z * nodeSize + NodeDiameter // in the center of where it needs to be.
						);
						var walkable = !Physics.CheckBox( worldPoint, boxCheck, Quaternion.identity, blockMask );
						var isGround = Physics.CheckBox( worldPoint, boxCheck, Quaternion.identity, groundMask );
						var gridPos  = new Vector3Int( x, y, z );

						// Locking the grid to make sure only this method is updating it.
						lock ( grid ) {
							grid[x, y, z] = new Node(
								worldPoint, // Where it exists within Unity.
								gridPos, // Where it logically exists on the grid.
								walkable, // If it is walkable or not.
								isGround // If the mesh is part of the ground.
							);
						}

						if ( Time.realtimeSinceStartup / Time.frameCount <= waitFramerate ) {
							continue;
						}

						Debug.Log( "Waiting until 24 FPS" );

						yield return new WaitForSecondsRealtime( waitFramerate );
					}
				}
			}

			IsGridReady = true;
		}

		/// <summary>
		/// Grabs the nodes that surround the node passed to it.
		/// </summary>
		/// <param name="node">The node to look around.</param>
		/// <remarks>
		/// The <see cref="IEnumerable{T}"/> max size is 26. The reason it is 26 is because (3^3)-1.
		/// 3^3 in case we want A* to go 3D, and we subtract 1 because it will contain <paramref name="node"/>.
		/// Since <paramref name="node"/> is already known, it can be excluded from the set.
		/// </remarks>
		/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Node"/>s that surround <paramref name="node"/>.</returns>
		private IEnumerable<NodePath> GetNeighborNodes( Node node ) {
			for ( var x = -1; x <= 1; ++x ) {
				var gridX = node.gridPos.x + x;

				if ( gridX < 0 || gridX >= gridSize.x ) {
					continue;
				}

				for ( var y = -1; y <= 1; ++y ) {
					var gridY = node.gridPos.y + y;

					if ( gridY < 0 || gridY >= gridSize.y ) {
						continue;
					}

					if ( node.IsGround && y < 0 ) {
						continue;
					}

					for ( var z = -1; z <= 1; ++z ) {
						var gridZ = node.gridPos.z + z;

						if ( gridZ < 0 || gridZ >= gridSize.z ) {
							continue;
						}

						if ( x == 0 && y == 0 && z == 0 ) {
							continue;
						}

						var gNode = grid[gridX, gridY, gridZ];

						// This should help keep execution flowing.
						yield return new NodePath( gNode );
					}
				}
			}
		}
	}
}
