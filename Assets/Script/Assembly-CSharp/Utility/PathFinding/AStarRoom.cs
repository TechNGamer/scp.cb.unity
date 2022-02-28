using UnityEngine;

namespace SCPCB.Remaster.Utility.PathFinding {
	public class AStarRoom {
		public Vector3 WorldPosition { get; }

		public Vector2Int GridPosition { get; }

		private float NodeDiameter {
			get {
				_nodeDiameter ??= nodeSize / 2f;

				return ( float )_nodeDiameter;
			}
		}

		private readonly float  nodeSize;
		private          float? _nodeDiameter;

		private readonly Vector3    worldSize;
		private readonly Vector3Int gridSize;
		private readonly LayerMask  ground, block;

		private AStarNode[,,] grid;

		internal AStarRoom( Vector3 worldSize, Vector3 worldPosition, LayerMask ground, LayerMask block, int gridX, int gridY, float nodeSize )
			: this( worldSize, worldPosition, new Vector2Int( gridX, gridY ), ground, block, nodeSize ) {
			// This constructor calls the other one, but makes the Vector2Int.
		}

		internal AStarRoom( Vector3 worldSize, Vector3 worldPosition, Vector2Int gridPos, LayerMask ground, LayerMask block, float nodeSize ) {
			// Properties are to be set here.
			WorldPosition = worldPosition;
			GridPosition  = gridPos;

			// Fields are to be set here.
			this.nodeSize  = nodeSize;
			this.worldSize = worldSize;
			this.ground    = ground;
			this.block     = block;

			// Calculations are done below this.
			var gridSizeX = Mathf.RoundToInt( worldSize.x / nodeSize );
			var gridSizeY = Mathf.RoundToInt( worldSize.y / nodeSize );
			var gridSizeZ = Mathf.RoundToInt( worldSize.z / nodeSize );

			gridSize = new Vector3Int( gridSizeX, gridSizeY, gridSizeZ );
		}

		#if UNITY_EDITOR
		internal void DrawGrid() {
			var size = new Vector3( 0.25f, 0.25f, 0.25f );
			
			for ( var x = 0; x < gridSize.x; ++x ) {
				for ( var y = 0; y < gridSize.y; ++y ) {
					for ( var z = 0; z < gridSize.z; ++z ) {
						var node = grid[x, y, z];

						if ( !node.IsWalkable ) {
							Gizmos.color = Color.red;
						} else if ( node.IsGround ) {
							Gizmos.color = Color.yellow;
						} else {
							Gizmos.color = Color.white;
						}
						
						Gizmos.DrawCube( node.WorldPosition, size );
					}
				}
			}
		}

		internal AStarNode[] FindPathInRoom( Vector3 start, Vector3 end ) {
			return null;
		}
		#endif

		// Initializes the room (child) grid.
		internal void CreateGrid() {
			var bottomLeft = WorldPosition - worldSize / 2f;
			var boxCheck   = new Vector3( nodeSize, nodeSize, nodeSize ) / 2f;

			grid = new AStarNode[gridSize.x, gridSize.y, gridSize.z];

			for ( var x = 0; x < gridSize.x; ++x ) {
				for ( var y = 0; y < gridSize.y; ++y ) {
					for ( var z = 0; z < gridSize.z; ++z ) {
						var worldPoint = bottomLeft + new Vector3(
							x * nodeSize + NodeDiameter,
							y * nodeSize + NodeDiameter,
							z * nodeSize + NodeDiameter
						);
						var walkable = !Physics.CheckBox( worldPoint, boxCheck, Quaternion.identity, block );
						var isGround = Physics.CheckBox( worldPoint, boxCheck, Quaternion.identity, ground );
						var gridPos  = new Vector3Int( x, y, z );

						grid[x, y, z] = new AStarNode(
							worldPoint,
							gridPos,
							walkable,
							isGround
						);
					}
				}
			}
		}

		// This object's very own method for getting the world point.
		// Why did I do this? So that way it is decoupled from the grid manager.
		private AStarNode GetNodeFromWorld( Vector3 worldPos ) {
			worldPos -= WorldPosition;

			var perX = ( worldPos.x + worldSize.x / 2 ) / worldSize.x;
			var perY = ( worldPos.y + worldSize.y / 2 ) / worldSize.y;
			var perZ = ( worldPos.z + worldSize.z / 2 ) / worldSize.z;

			if ( IsOutsideBounds( perX ) || IsOutsideBounds( perY ) || IsOutsideBounds( perZ ) ) {
				throw new PointOutOfAreaException( worldPos );
			}

			var x = Mathf.RoundToInt( perX * ( gridSize.x - 1 ) );
			var y = Mathf.RoundToInt( perY * ( gridSize.y - 1 ) );
			var z = Mathf.RoundToInt( perZ * ( gridSize.z - 1 ) );

			return grid[x, y, z];

			static bool IsOutsideBounds( float v ) => v < 0f || v > 1f;
		}
	}
}
