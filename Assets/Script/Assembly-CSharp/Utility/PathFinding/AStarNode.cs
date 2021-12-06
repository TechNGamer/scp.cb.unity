using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SCPCB.Remaster.Utility.PathFinding {
	/// <summary>
	/// Represents a node on the grid system.
	/// </summary>
	[SuppressMessage( "ReSharper", "MemberCanBePrivate.Global" )]
	[SuppressMessage( "ReSharper", "UnusedMember.Global" )]
	public class AStarNode : IEquatable<AStarNode> {
		/// <summary>
		/// Checks to see if the nodes are the same.
		/// </summary>
		/// <param name="l">The left side of the operand.</param>
		/// <param name="r">The right side of the operand.</param>
		/// <returns>True if they are the same.</returns>
		public static bool operator ==( AStarNode l, AStarNode r ) => !ReferenceEquals( l, null ) && l.Equals( r );

		/// <summary>
		/// Checks to see if the nodes are not the same.
		/// </summary>
		/// <param name="l">The left side of the operand.</param>
		/// <param name="r">The right side of the operand.</param>
		/// <returns>True if they are not the same.</returns>
		public static bool operator !=( AStarNode l, AStarNode r ) => !( l == r );

		/// <summary>
		/// The X position it is located at in the world.
		/// </summary>
		public float WorldX => WorldPosition.x;

		/// <summary>
		/// The Y location of where it is at in the world.
		/// </summary>
		public float WorldY => WorldPosition.y;

		/// <summary>
		/// The Z location of where it is at in the world.
		/// </summary>
		public float WorldZ => WorldPosition.z;

		/// <summary>
		/// If this node is walkable or not.
		/// </summary>
		public bool IsWalkable { get; }

		/// <summary>
		/// If this node is on the ground or not.
		/// </summary>
		public bool IsGround { get; }

		/// <summary>
		/// The world position of the node.
		/// </summary>
		public Vector3    WorldPosition { get; }
		
		/// <summary>
		/// The position it resides within the grid.
		/// </summary>
		public Vector3Int GridPos       { get; }

		// This is internal because nothing outside the library should be creating them.
		internal AStarNode( Vector3 worldPos, Vector3Int gridPos, bool isWalkable, bool isGround = true ) {
			IsWalkable    = isWalkable;
			IsGround      = isGround;
			WorldPosition = worldPos;
			GridPos       = gridPos;
		}

		/// <summary>
		/// Compares to see if the current node is the same as the other node.
		/// </summary>
		/// <param name="other">Another <see cref="AStarNode"/>.</param>
		/// <returns>True if they are the same.</returns>
		public bool Equals( AStarNode other ) {
			if ( ReferenceEquals( null, other ) ) {
				return false;
			}

			if ( ReferenceEquals( this, other ) ) {
				return true;
			}

			return GetHashCode() == other.GetHashCode();
		}

		/// <summary>
		/// Compares to see if the <paramref name="obj"/> is the current AStarNode.
		/// </summary>
		/// <param name="obj">An <see cref="object"/> to compare the current node against.</param>
		/// <returns>True if they are the same, otherwise false.</returns>
		public override bool Equals( object obj ) {
			if ( ReferenceEquals( null, obj ) ) {
				return false;
			}

			if ( ReferenceEquals( this, obj ) ) {
				return true;
			}

			return obj.GetType() == GetType() && Equals( ( AStarNode )obj );
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = IsWalkable.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ WorldPosition.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ GridPos.GetHashCode();
				return hashCode;
			}
		}
	}
}
