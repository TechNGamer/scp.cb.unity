using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SCPCB.Remaster.Utility {
	public class AStarGridManager : MonoBehaviour {
		[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
		public struct Node {
			public bool walkable;
			public int  gCost;
			public int  hCost;

			public Vector3    worldPosition;
			public Vector2Int gridPos;

			public int FCost => gCost + hCost;

			public Node( bool walkable, Vector3 worldPos, Vector2Int gridPos ) {
				this.walkable = walkable;
				worldPosition = worldPos;
				this.gridPos  = gridPos;
				gCost         = 0;
				hCost         = 0;
			}
		}

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

		public Node GetNodeFromWorld( Vector3 position ) {
			var perX = Mathf.Clamp01( ( position.x + gridSize.x / 2 ) / gridSize.x );
			var perY = Mathf.Clamp01( ( position.z + gridSize.y / 2 ) / gridSize.y );
			var x    = Mathf.RoundToInt( perX * ( grid.GetLength( 0 ) - 1 ) );
			var y    = Mathf.RoundToInt( perY * ( grid.GetLength( 1 ) - 1 ) );

			return grid[x, y];
		}

		public Node[] FindPath( Vector3 start, Vector3 end ) {
			var startNode = GetNodeFromWorld( start );
			var endNode   = GetNodeFromWorld( end );
			var openSet   = new List<Node>();
			var closedSet = new HashSet<Node>();

			openSet.Add( startNode );

			return null;

			while ( openSet.Count > 0 ) {
			}
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

		private Node?[] GetNeighborNodes( Node node ) {
			var list  = new Node?[8];
			var index = 0;

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

					list[index++] = gNode;
				}
			}

			return list;
		}
	}
}
