using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace QuikGraph.Collections {
    /// <summary>
    /// Represents a list of <see cref="FibonacciHeapCell{TPriority,TValue}"/>.
    /// </summary>
    /// <typeparam name="TPriority">Priority type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    [Serializable]
    public sealed class FibonacciHeapLinkedList<TPriority, TValue> :
        IEnumerable<FibonacciHeapCell<TPriority, TValue>> {

        
        private FibonacciHeapCell<TPriority, TValue> _last;

        /// <summary>
        /// First <see cref="FibonacciHeapCell{TPriority,TValue}"/>.
        /// </summary>
        
        public FibonacciHeapCell<TPriority, TValue> First { get; private set; }

        internal FibonacciHeapLinkedList() {
            First = null;
            _last = null;
        }

        /// <summary>
        /// Merges the given <paramref name="cells"/> at the end of this cells list.
        /// </summary>
        /// <param name="cells">Cells to merge.</param>
        internal void MergeLists(
             FibonacciHeapLinkedList<TPriority, TValue> cells
        ) {
            Debug.Assert(cells != null);

            if (cells.First is null)
                return;

            if (_last != null)
            {
                _last.Next = cells.First;
            }

            cells.First.Previous = _last;
            _last = cells._last;

            if (First is null)
            {
                First = cells.First;
            }
        }

        /// <summary>
        /// Adds the given <paramref name="cell"/> at the end of this cells list.
        /// </summary>
        /// <param name="cell">Cell to add.</param>
        internal void AddLast(
             FibonacciHeapCell<TPriority, TValue> cell
        ) {
            Debug.Assert(cell != null);

            if (_last != null)
            {
                _last.Next = cell;
            }

            cell.Previous = _last;
            _last = cell;

            if (First is null)
            {
                First = cell;
            }
        }

        /// <summary>
        /// Removes the given <paramref name="cell"/> from this cells list.
        /// </summary>
        /// <param name="cell">Cell to remove.</param>
        internal void Remove(
             FibonacciHeapCell<TPriority, TValue> cell
        ) {
            Debug.Assert(cell != null);

            if (cell.Previous != null)
            {
                cell.Previous.Next = cell.Next;
            }
            else if (First == cell)
            {
                First = cell.Next;
            }

            if (cell.Next != null)
            {
                cell.Next.Previous = cell.Previous;
            }
            else if (_last == cell)
            {
                _last = cell.Previous;
            }

            cell.Next = null;
            cell.Previous = null;
        }

        #region IEnumerable

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region IEnumerable<FibonacciHeapNode<TPriority,TValue>>

        /// <inheritdoc />
        public
            IEnumerator<FibonacciHeapCell<TPriority, TValue>> GetEnumerator() {
            FibonacciHeapCell<TPriority, TValue> current = First;
            
            while (current != null) {
                yield return current;
                current = current.Next;
            }
        }

        #endregion
    }
}