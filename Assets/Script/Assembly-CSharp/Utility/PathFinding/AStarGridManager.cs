using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using SCPCB.Remaster.Data;
using UnityEngine;

namespace SCPCB.Remaster.Utility.PathFinding {
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
		// This class is meant to be thread safe, along with call agnostic.
		[SuppressMessage( "ReSharper", "NonReadonlyMemberInGetHashCode" )]
		[SuppressMessage( "ReSharper", "MemberCanBePrivate.Local" )]
		private class NodePath : IEquatable<AStarNode>, IEquatable<NodePath>, Heap<NodePath>.IHeapItem<NodePath> {
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

			public static implicit operator AStarNode( NodePath n ) => n.aStarNode;

			// Object
			public int HeapIndex { get; set; }

			// Is meant as a quick access.
			public bool IsWalkable => aStarNode.IsWalkable;

			public bool IsGround => aStarNode.IsGround;

			public int FCost => gCost + hCost;

			public Vector3 WorldPosition => aStarNode.WorldPosition;

			public int gCost;
			public int hCost;

			public readonly AStarNode aStarNode;
			public          NodePath  parent;

			// The constructor helps ensures that a node was passed to it.
			public NodePath( AStarNode aStarNode ) {
				if ( aStarNode == null ) {
					throw new ArgumentNullException(
						nameof( aStarNode ),
						"A node must be provided in order to build a proper path."
					);
				}

				this.aStarNode = aStarNode;
			}

			// This method will throw an exception if the node is null, since this object's hash is the same as the nodes.
			public override int GetHashCode() {
				if ( aStarNode == null ) {
					throw new NullReferenceException( "Node must be non-null to generate hash." );
				}

				return aStarNode.GetHashCode();
			}

			public bool Equals( AStarNode other ) => Equals( ( object )other );

			public bool Equals( NodePath other ) => Equals( ( object )other );

			public int CompareTo( NodePath other ) {
				var compareNum = FCost.CompareTo( other.FCost );

				return compareNum == 0 ? hCost.CompareTo( other.hCost ) : -compareNum;
			}

			public override bool Equals( object obj ) =>
				obj switch {
					NodePath np => aStarNode == np.aStarNode,
					AStarNode n => aStarNode == n,
					_           => ReferenceEquals( this, obj )
				};

			public override string ToString() => WorldPosition.ToString();
		}

		private class AStarRoomPath : IEquatable<AStarRoomPath>, Heap<AStarRoomPath>.IHeapItem<AStarRoomPath> {
			public int FCost => GCost + HCost;

			public int GCost { get; set; }

			public int HCost { get; set; }

			public AStarRoom Room { get; }

			public AStarRoomPath( AStarRoom room ) {
				Room = room;
			}

			public bool Equals( AStarRoomPath other ) => Equals( ( object )other );

			public int CompareTo( AStarRoomPath other ) {
				var compareNum = FCost.CompareTo( other.FCost );

				return compareNum == 0 ? HCost.CompareTo( other.HCost ) : -compareNum;
			}

			public int HeapIndex { get; set; }
		}
		#endregion

		// Everything that is static of this class belongs here.
		#region Static
		/* As the method name says, it get's the distance between 2 nodes.
		 * However, it also adds some weights to the values so they are not small. */
		private static int GetDistance( AStarNode aStarNodeA, AStarNode aStarNodeB ) {
			var distX = Mathf.Abs( aStarNodeA.GridPos.x - aStarNodeB.GridPos.x );
			var distY = Mathf.Abs( aStarNodeA.GridPos.y - aStarNodeB.GridPos.y );
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

		/// <summary>
		/// If the grid is ready for calls or not.
		/// </summary>
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
		[Tooltip( "The size of the grid to make." )]
		private Vector3 numOfRooms;

		[SerializeField]
		[Tooltip( "The size of each room." )]
		private Vector3 roomSize;

		private Vector2Int gridSize;
		private Vector3    position;

		private AStarRoom[,] grid;

		#region Unity Events
		private void Awake() {
			// var gridSizeX = Mathf.RoundToInt( numOfRooms.x / roomSize.x );
			// var gridSizeY = Mathf.RoundToInt( numOfRooms.z / roomSize.z );
			var gridSizeX = ( int )numOfRooms.x;
			var gridSizeY = ( int )numOfRooms.z;

			gridSize = new Vector2Int( gridSizeX, gridSizeY );

			// Since the grid should never move from where it is placed, we can do this and now that worldPos is accessible on any thread.
			position = transform.position;

			StartCoroutine( CreateGrid( gridSizeX, gridSizeY ) );
		}

		private void Reset() => Awake();

		[SuppressMessage( "ReSharper", "LocalVariableHidesMember" )]
		private void OnDrawGizmosSelected() {
			if ( numOfRooms == Vector3.zero ) {
				return;
			}

			var position = transform.position;

			Gizmos.DrawWireCube( position, numOfRooms );

			if ( grid == null || !IsGridReady || grid.Length == 0 ) {
				ShowDynamicRoomGrid();
			} else {
				ShowGrid();
			}

			void ShowGrid() {
				if ( !IsGridReady ) {
					return;
				}

				for ( var x = 0; x < gridSize.x; ++x ) {
					for ( var y = 0; y < gridSize.y; ++y ) {
						var room = grid[x, y];

						Gizmos.color = new Color(
							x / ( float )( gridSize.x ),
							0f,
							y / ( float )( gridSize.y )
						);

						Gizmos.DrawWireCube( room.WorldPosition, roomSize );

						if ( !showGrid ) {
							continue;
						}

						try {
							room.DrawGrid();
						} catch {
							// We don't care.
						}
					}
				}
			}

			// Generates fake rooms to show where it might be located at.
			void ShowDynamicRoomGrid() {
				var gridSizeX = numOfRooms.x;
				var gridSizeZ = numOfRooms.z;

				for ( var x = 0; x < gridSizeX; ++x ) {
					for ( var y = 0; y < gridSizeZ; ++y ) {
						var worldPos = new Vector3(
							x * roomSize.x + roomSize.x / 2f,
							roomSize.y / 2f,
							y * roomSize.z + roomSize.z / 2f
						);
						var localBottomLeft = worldPos - roomSize / 2f;

						Gizmos.color = new Color(
							x / gridSizeX,
							0f,
							y / gridSizeZ
						);

						Gizmos.DrawWireCube( worldPos, roomSize );
						Gizmos.DrawWireSphere( localBottomLeft, 0.5f );
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// This method uses the world position to calculate where on the grid it is.
		/// </summary>
		/// <param name="worldPos">The world position of the object.</param>
		/// <returns>The <see cref="AStarNode"/> that represents that portion of the world.</returns>
		// ReSharper disable once MemberCanBePrivate.Global
		private AStarRoom GetRoomFromWorld( Vector3 worldPos ) {
			// Makes an obsolete position a relative one. Which makes it good to use anywhere.
			worldPos -= position;

			/* Getting the logical position of it's location on the grid.
			 * It does this by adding the world position to half of the grid world size.
			 * It then divides that by the grid world size to get where it is between 0 and 1. */
			var perX = ( worldPos.x + numOfRooms.x / 2 ) / numOfRooms.x;
			var perY = ( worldPos.y + numOfRooms.y / 2 ) / numOfRooms.y;

			/* Instead of clamping the value, I think it would be better to alert that the value is out of bounds.
			 * Since I plan on allowing mods, it only makes sense to allow the mod developers into knowing why their
			 * code is failing or what they are doing wrong. */
			if ( IsOutsideBounds( perX ) || IsOutsideBounds( perY ) ) {
				throw new PointOutOfAreaException( worldPos );
			}

			/* Get's the XY coords by multiplying the grid size by the percentage, then subtracting 1.
			 * The reason for not using GridWorldSize for this is that it needs to know how big the grid
			 * it, so it can get the general location. */
			var x = Mathf.RoundToInt( perX * ( gridSize.x - 1 ) );
			var y = Mathf.RoundToInt( perY * ( gridSize.y - 1 ) );

			// Returns that node at that position.
			return grid[x, y];

			static bool IsOutsideBounds( float v ) => v < 0f || v > 1f;
		}

		/// <summary>
		/// Same as <see cref="FindPath"/>, but does it on another Task.
		/// </summary>
		/// <param name="start">The starting world position to calculate from.</param>
		/// <param name="end">The end world position.</param>
		/// <param name="token">The token to cancel the job.</param>
		/// <param name="canFly">If the object can go into the air or not.</param>
		/// <returns>An array of nodes to follow.</returns>
		public Task<AStarNode[]> FindPathAsync( Vector3 start, Vector3 end, CancellationToken token, bool canFly = false ) => Task.Run( () => {
			try {
				return FindPath( start, end, canFly );
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
		/// <param name="canFly">If the object can go into the air or not.</param>
		/// <returns>An array of nodes of the path.</returns>
		/// <exception cref="Exception">You should never see this. If you do, something went horrible wrong.</exception>
		[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
		public AStarNode[] FindPath( Vector3 start, Vector3 end, bool canFly = false ) {
			if ( !IsGridReady ) {
				return null;
			}

			// var startNode  = new NodePath( GetNodeFromWorld( start ) );
			// var targetNode = new NodePath( GetNodeFromWorld( end ) );
			// var openSet    = new Heap<NodePath>( grid.Length );
			// var closedSet  = new HashSet<NodePath>();
			//
			// openSet.Add( startNode );
			//
			// while ( openSet.Count > 0 ) {
			// 	var currentNode = openSet.RemoveFirst();
			// 	closedSet.Add( currentNode );
			//
			// 	// This is the first time where a LinkedList actually comes in handy.
			// 	// This method basically created a LinkedList, but in one direction. Using C#'s built in LinkedList means that we can `AddFirst` the parents.
			// 	// Which means that we are reversing the list while also adding it to another. Then we use Linq to return an array.
			// 	if ( currentNode == targetNode ) {
			// 		var list = new LinkedList<AStarNode>();
			// 		var node = currentNode;
			//
			// 		do {
			// 			list.AddFirst( node.aStarNode );
			//
			// 			node = node.parent;
			// 		} while ( node != null );
			//
			// 		return list.ToArray();
			// 	}
			//
			// 	foreach ( var neighborNode in GetNeighborNodes( currentNode ) ) {
			// 		if ( !neighborNode.IsGround && !canFly || !neighborNode.IsWalkable || closedSet.Contains( neighborNode ) ) {
			// 			continue;
			// 		}
			//
			// 		var newMoveCost = currentNode.gCost + GetDistance( currentNode, neighborNode );
			//
			// 		if ( newMoveCost > neighborNode.gCost && openSet.Contains( neighborNode ) ) {
			// 			continue;
			// 		}
			//
			// 		neighborNode.gCost  = newMoveCost;
			// 		neighborNode.hCost  = GetDistance( neighborNode, targetNode );
			// 		neighborNode.parent = currentNode;
			//
			// 		if ( !openSet.Contains( neighborNode ) ) {
			// 			openSet.Add( neighborNode );
			// 		}
			// 	}
			// }

			return null;
		}

		private IEnumerator CreateGrid( int sizeX, int sizeY ) {
			const float WAIT_FRAMERATE = 1f / 24f;
			var         bottomLeft     = transform.position - numOfRooms / 2f;

			yield return new WaitForEndOfFrame();

			// Since the path finding is done via A*, the best way to calculate the path is to make a grid.
			// There is probably a better way to do this but for now the grid will have to do.
			grid = new AStarRoom[sizeX, sizeY];

			// This is the part that is of course going to take the longest. Should probably put it on a different task.
			for ( var x = 0; x < sizeX; ++x ) {
				for ( var z = 0; z < sizeY; ++z ) {
					/* We times each X-Z iteration by the room size in that direction and add half the room size to center it.
					 * The Y position is half the height since it starts at the bottom left, which is half the size down.
					 */
					var worldPos = bottomLeft + new Vector3(
						x * roomSize.x + roomSize.x / 2f,
						roomSize.y / 2f,
						z * roomSize.z + roomSize.z / 2f
					);
					var room = new AStarRoom( roomSize, worldPos, groundMask, blockMask, x, z, nodeSize );

					grid[x, z] = room;

					room.CreateGrid();

					if ( Time.realtimeSinceStartup / Time.frameCount <= WAIT_FRAMERATE ) {
						continue;
					}

					Debug.Log( "Waiting until 24 FPS" );

					yield return new WaitForSecondsRealtime( WAIT_FRAMERATE );
				}
			}

			IsGridReady = true;
		}

		/// <summary>
		/// Grabs the nodes that surround the node passed to it.
		/// </summary>
		/// <param name="room">The node to look around.</param>
		/// <remarks>
		/// The <see cref="IEnumerable{T}"/> max size is 26. The reason it is 26 is because (3^3)-1.
		/// 3^3 in case we want A* to go 3D, and we subtract 1 because it will contain <paramref name="room"/>.
		/// Since <paramref name="room"/> is already known, it can be excluded from the set.
		/// </remarks>
		/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="AStarRoom"/>s that surround <paramref name="room"/>.</returns>
		private IEnumerable<AStarRoomPath> GetNeighborNodes( AStarRoom room ) {
			for ( var x = -1; x <= 1; ++x ) {
				var gridX = room.GridPosition.x + x;

				if ( gridX < 0 || gridX >= gridSize.x ) {
					continue;
				}

				for ( var y = -1; y <= 1; ++y ) {
					var gridY = room.GridPosition.y + y;

					if ( gridY < 0 || gridY >= gridSize.y ) {
						continue;
					}

					if ( x == 0 && y == 0 ) {
						continue;
					}

					var gNode = grid[gridX, gridY];

					yield return new AStarRoomPath( grid[x, y] );
				}
			}
		}
	}
}
