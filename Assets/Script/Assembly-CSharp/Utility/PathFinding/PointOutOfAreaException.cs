using System;
using System.Text;
using UnityEngine;

namespace SCPCB.Remaster.Utility.PathFinding {
	/// <summary>
	/// Used to denote that a point is not inside an area.
	/// </summary>
	/// <remarks>
	/// This <seealso cref="Exception"/> is raised when an entity is attempting to use path finding outside the area.
	/// Since clamping could be used, it might have unintended side effects. Namely that of entities getting stuck out
	/// of bounds or stuck in general.
	/// </remarks>
	public class PointOutOfAreaException : Exception {
		
		/// <summary>
		/// The point which caused the error.
		/// </summary>
		public Vector3 Point { get; }

		/// <summary>
		/// Used to construct the exception just from the point of error.
		/// </summary>
		/// <param name="point">The point where this occured.</param>
		public PointOutOfAreaException( Vector3 point ) : this( "A requested point is outside the allowed area.", point ) {
		}

		/// <summary>
		/// Used to construct the exception with a custom message, and optionally, an inner exception if that is the reason.
		/// </summary>
		/// <param name="message">The custom message to use.</param>
		/// <param name="point">The point where this occured.</param>
		/// <param name="inner">An optional inner exception.</param>
		public PointOutOfAreaException( string message, Vector3 point, Exception inner = null ) : base( message, inner ) {
			Point = point;
		}

		/// <summary>
		/// Creates a string that represents this exception, plus any inner exceptions.
		/// </summary>
		/// <returns>The message this exception carries.</returns>
		public override string ToString() {
			var builder = new StringBuilder();
			// Since this object is also an exception, might as will just cast itself to an exception.
			var e       = ( Exception )this;
			var depth   = 0;

			// Loops over every exception, then assigns the inner to e for checking.
			// This is a do-while since the first exception to print is the object itself, and thus is definitely not null.
			do {
				builder.Append( '\t', depth ).Append( "Type: " ).AppendLine( e.GetType().FullName )
					.Append( '\t', depth ).Append( "Message: " ).AppendLine( Message )
					.Append( '\t', depth ).AppendLine( "Stacktrace:" ).Append( '\t', depth )
					.AppendLine( StackTrace );

				e = e.InnerException;

				++depth;
			} while ( e != null );

			// Trimming to make sure there is no whitespace, since it irritates me when there is whitespace after a string.
			return builder.ToString().Trim();
		}
	}
}
