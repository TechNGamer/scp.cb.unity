using System;

namespace SCPCB.Remaster.Data {
	public class Heap<T> where T : Heap<T>.IHeapItem<T> {
		#pragma warning disable 693
		public interface IHeapItem<in T> : IComparable<T> {
			#pragma warning restore 693
			public int HeapIndex { get; set; }
		}

		public int Count { get; private set; }

		private readonly T[] items;

		public Heap( int maxHeapSize ) {
			items = new T[maxHeapSize];
		}

		public void Add( T item ) {
			items[Count++] = item;

			SortUp( item );
		}

		public T Remove() {
			var first = items[0];

			--Count;

			items[0]           = items[Count];
			items[0].HeapIndex = 0;

			SortDown( first );

			return first;
		}

		public bool Contains( T item ) {
			foreach ( var heapItem in items ) {
				if ( Equals( heapItem, item ) ) {
					return true;
				}
			}

			return false;
		}

		public void Update( T item ) {
			SortUp( item );
			SortDown( item );
		}

		private void SortDown( T item ) {
			while ( true ) {
				var childLeft  = item.HeapIndex * 2 + 1;
				var childRight = item.HeapIndex * 2 + 2;

				if ( childLeft >= Count ) {
					break;
				}

				var swapIndex = childLeft;

				if ( childRight < Count && items[childLeft].CompareTo( items[childRight] ) < 0 ) {
					swapIndex = childRight;
				}

				if ( item.CompareTo( items[swapIndex] ) < 0 ) {
					Swap( item, items[swapIndex] );
				} else {
					break;
				}
			}
		}

		private void SortUp( T item ) {
			var parentIndex = ( item.HeapIndex - 1 ) / 2;

			while ( true ) {
				var parentItem = items[parentIndex];

				if ( item.CompareTo( parentItem ) > 0 ) {
					Swap( item, parentItem );
				} else {
					break;
				}
			}
		}

		private void Swap( T a, T b ) {
			var bIndex = b.HeapIndex;

			items[a.HeapIndex] = b;
			items[b.HeapIndex] = a;

			b.HeapIndex = a.HeapIndex;
			a.HeapIndex = bIndex;
		}
	}
}
