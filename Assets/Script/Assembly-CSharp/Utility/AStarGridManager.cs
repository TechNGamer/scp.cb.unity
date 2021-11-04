using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPCB.Remaster.Utility {
	public class AStarGridManager : MonoBehaviour {
		[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
		public class Node : IEquatable<Node> {
			public float WorldX => worldPosition.x;
			public float WorldY => worldPosition.y;
			public float WorldZ => worldPosition.z;

			public readonly bool walkable;

			internal readonly Vector3    worldPosition;
			internal readonly Vector2Int gridPos;

			public Node( bool walkable, Vector3 worldPos, Vector2Int gridPos ) {
				this.walkable = walkable;
				worldPosition = worldPos;
				this.gridPos  = gridPos;
			}

			public bool Equals( Node other ) {
				if ( ReferenceEquals( null, other ) )
					return false;
				if ( ReferenceEquals( this, other ) )
					return true;
				return walkable == other.walkable && worldPosition.Equals( other.worldPosition ) && gridPos.Equals( other.gridPos );
			}

			public override bool Equals( object obj ) {
				if ( ReferenceEquals( null, obj ) )
					return false;
				if ( ReferenceEquals( this, obj ) )
					return true;
				if ( obj.GetType() != this.GetType() )
					return false;
				return Equals( ( Node )obj );
			}

			public override int GetHashCode() {
				unchecked {
					var hashCode = walkable.GetHashCode();
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
		private class NodePath : IEquatable<Node>, IEquatable<NodePath> {
			public static bool operator ==( NodePath l, NodePath r ) {
				var leftNull  = ReferenceEquals( l, null );
				var rightNull = ReferenceEquals( r, null );

				if ( leftNull && rightNull ) {
					return true;
				}

				if ( leftNull ^ !rightNull ) {
					return false;
				}

				return l.Equals( r );
			}

			public static bool operator !=( NodePath l, NodePath r ) => !( l == r );

			public static implicit operator Node( NodePath n ) => n.node;

			// Object

			public bool Walkable => node.walkable;

			public int FCost => gCost + hCost;

			public Vector3 WorldPosition => node.worldPosition;


			public int gCost;
			public int hCost;

			public readonly Node     node;
			public          NodePath parent;

			public NodePath( Node node ) {
				this.node = node;
			}

			public override int GetHashCode() {
				if ( node == null ) {
					throw new NullReferenceException( "Node must be non-null to generate hash." );
				}

				return node.GetHashCode();
			}

			public bool Equals( Node other ) => Equals( ( object )other );

			public bool Equals( NodePath other ) => Equals( ( object )other );

			public override bool Equals( object obj ) =>
				obj switch {
					NodePath np => node == np.node,
					Node n      => node == n,
					_           => ReferenceEquals( this, obj )
				};

			public override string ToString() => WorldPosition.ToString();
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public float NodeDiameter => nodeSize / 2f;

		[SerializeField]
		[Tooltip( "How big each node is." )]
		private float nodeSize;

		[SerializeField]
		[Tooltip( "The meshes on the grid that cannot be walked through." )]
		private LayerMask blockMask;

		[SerializeField]
		[Tooltip( "The size of the grid to make." )]
		private Vector2 gridSize;

		private Node[,] grid;

		// ReSharper disable once MemberCanBePrivate.Global
		public Node GetNodeFromWorld( Vector3 position ) {
			var perX = Mathf.Clamp01( ( position.x + gridSize.x / 2 ) / gridSize.x );
			var perY = Mathf.Clamp01( ( position.z + gridSize.y / 2 ) / gridSize.y );
			var x    = Mathf.RoundToInt( perX * ( grid.GetLength( 0 ) - 1 ) );
			var y    = Mathf.RoundToInt( perY * ( grid.GetLength( 1 ) - 1 ) );

			return grid[x, y];
		}

		/// <summary>
		/// Same as <see cref="FindPath"/>, but does it on another Task.
		/// </summary>
		/// <param name="start">The starting world position to calculate from.</param>
		/// <param name="end">The end world position.</param>
		/// <param name="token">The token to cancel the job.</param>
		/// <returns>An array of nodes to follow.</returns>
		public Task<Node[]> FindPathAsync( Vector3 start, Vector3 end, CancellationToken token ) => Task.Run( () => FindPath( start, end ), token );

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
			var startNode  = new NodePath( GetNodeFromWorld( start ) );
			var targetNode = new NodePath( GetNodeFromWorld( end ) );
			var openSet    = new List<NodePath>();
			var closedSet  = new HashSet<NodePath>();

			openSet.Add( startNode );

			while ( openSet.Count > 0 ) {
				var currentNode = openSet[0];

				for ( var i = 1; i < openSet.Count; ++i ) {
					if ( openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost ) {
						currentNode = openSet[i];
					}
				}

				openSet.Remove( currentNode );
				closedSet.Add( currentNode );

				// This is the first time where a LinkedList actually comes in handy.
				// This method basically created a LinkedList, but in one direction. Using C#'s built in LinkedList means that we can `AddFirst` the parents.
				// Which means that we are reversing the list while also adding it to another. Then we use Linq to return an array.
				if ( currentNode == targetNode ) {
					var list = new LinkedList<Node>();
					var node = targetNode;

					do {
						list.AddFirst( node.node );

						#if UNITY_EDITOR
						if ( node.parent != null ) {
							// This is just to debug the path finding.
							Debug.DrawLine( node.WorldPosition, node.parent.WorldPosition, Color.red );
						}
						#endif

						node = node.parent;
					} while ( node != null );

					return list.ToArray();
				}

				foreach ( var neighborNode in GetNeighborNodePaths( currentNode ) ) {
					if ( !neighborNode.Walkable || closedSet.Contains( neighborNode ) ) {
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

			throw new Exception( "Something happened that shouldn't have." );
		}

		private void Awake() {
			var gridSizeX = Mathf.RoundToInt( gridSize.x / ( nodeSize / 1f ) );
			var gridSizeY = Mathf.RoundToInt( gridSize.y / ( nodeSize / 1f ) );

			CreateGrid( gridSizeX, gridSizeY );
		}

		private void Reset() => Awake();

		private void OnDrawGizmos() {
			if ( gridSize == Vector2.zero ) {
				return;
			}

			var position = transform.position;

			Gizmos.DrawWireCube( position, new Vector3( gridSize.x, 1f, gridSize.y ) );

			// Bottom Left
			Gizmos.color = Color.black;

			var bottomLeft = position - new Vector3( gridSize.x, 0f, gridSize.y ) / 2f;
			Gizmos.DrawWireSphere( bottomLeft, 1f );

			if ( grid == null ) {
				return;
			}

			for ( var x = 0; x < grid.GetLength( 0 ); ++x ) {
				for ( var y = 0; y < grid.GetLength( 1 ); ++y ) {
					Gizmos.color = grid[x, y].walkable ? Color.white : Color.red;

					Gizmos.DrawCube( grid[x, y].worldPosition, new Vector3( nodeSize, 0.25f, nodeSize ) );
				}
			}
		}

		private void CreateGrid( int sizeX, int sizeY ) {
			var bottomLeft = transform.position - new Vector3( gridSize.x, 0f, gridSize.y ) / 2f;
			var boxCheck   = new Vector3( nodeSize, 0.5f, nodeSize ) / 2f;

			grid = new Node[sizeX, sizeY];

			for ( var x = 0; x < sizeX; ++x ) {
				for ( var y = 0; y < sizeY; ++y ) {
					var worldPoint = bottomLeft + new Vector3( x * nodeSize + NodeDiameter, 0.5f, y * nodeSize + NodeDiameter );
					var notBlocked = !Physics.CheckBox( worldPoint, boxCheck, Quaternion.identity, blockMask );

					grid[x, y] = new Node( notBlocked, worldPoint, new Vector2Int( x, y ) );
				}
			}
		}

		private int GetDistance( Node nodeA, Node nodeB ) {
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
		private IEnumerable<NodePath> GetNeighborNodePaths( Node node ) {
			for ( var x = -1; x <= 1; ++x ) {
				var gridX = node.gridPos.x + x;

				if ( gridX < 0 || gridX >= grid.GetLength( 0 ) ) {
					continue;
				}

				for ( var y = -1; y <= 1; ++y ) {
					if ( x == 0 && y == 0 ) {
						continue;
					}

					var gridY = node.gridPos.y + y;

					if ( gridY < 0 || gridY >= grid.GetLength( 1 ) ) {
						continue;
					}

					var gNode = grid[gridX, gridY];

					// Since we can just yield return this, all that is created is a state machine every method call, and struct on every resume.
					yield return new NodePath( gNode );
				}
			}
		}
	}
}
