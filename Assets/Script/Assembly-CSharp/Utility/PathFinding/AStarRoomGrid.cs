using System.Threading.Tasks;
using UnityEngine;

namespace SCPCB.Remaster.Utility.PathFinding {
	internal class AStarRoomGrid {
		public Vector3 WorldPosition { get; }

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

		internal AStarRoomGrid( Vector3 worldSize, Vector3 worldPosition, LayerMask ground, LayerMask block, float nodeSize ) {
			WorldPosition = worldPosition;

			this.nodeSize  = nodeSize;
			this.worldSize = worldSize;
			this.ground    = ground;
			this.block     = block;

			var gridSizeX = Mathf.RoundToInt( worldSize.x / nodeSize );
			var gridSizeY = Mathf.RoundToInt( worldSize.y / nodeSize );
			var gridSizeZ = Mathf.RoundToInt( worldSize.z / nodeSize );

			gridSize = new Vector3Int( gridSizeX, gridSizeY, gridSizeZ );
		}

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
	}
}
