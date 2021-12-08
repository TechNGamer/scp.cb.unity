using System;
using System.Text;
using UnityEngine;

namespace SCPCB.Remaster.Utility.PathFinding {
	public class PointOutOfAreaException : Exception {
		public Vector3 Point { get; }

		public PointOutOfAreaException( Vector3 point ) : this( "A requested point is outside the allowed area.", point ) {
		}

		public PointOutOfAreaException( string message, Vector3 point, Exception inner = null ) : base( message, inner ) {
			Point = point;
		}

		public override string ToString() {
			var builder = new StringBuilder();
			var e       = ( Exception )this;
			var depth   = 0;

			while ( e != null ) {
				builder.Append( '\t', depth ).Append( "Type: " ).AppendLine( e.GetType().FullName )
					.Append( '\t', depth ).Append( "Message: " ).AppendLine( Message )
					.Append( '\t', depth ).AppendLine( "Stacktrace:" ).Append( '\t', depth )
					.AppendLine( StackTrace );

				e = e.InnerException;

				++depth;
			}

			return builder.ToString().Trim();
		}
	}
}
