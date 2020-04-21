using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuikGraph.Algorithms;


namespace QuikGraph.Collections {
    /// <summary>
    /// Priority queue to sort vertices by distance priority (use <see cref="FibonacciHeap{TPriority,TValue}"/>).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TDistance">Distance type.</typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public sealed class FibonacciQueue<TVertex, TDistance> :
        IPriorityQueue<TVertex> {

        
        private readonly Func<TVertex, TDistance> _distanceFunc;

        
        private readonly FibonacciHeap<TDistance, TVertex> _heap;

        
        private readonly
            Dictionary<TVertex, FibonacciHeapCell<TDistance, TVertex>> _cells;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryQueue{TVertex,TDistance}"/> class.
        /// </summary>
        /// <param name="distanceFunc">Function that compute the distance for a given vertex.</param>
        public FibonacciQueue(
             Func<TVertex,
            TDistance> distanceFunc
        ) : this(
                0,
                null,
                distanceFunc,
                Comparer<TDistance>.Default.Compare
            ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibonacciQueue{TVertex,TDistance}"/> class.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        /// <param name="values">Set of vertices (null if <paramref name="capacity"/> is 0).</param>
        /// <param name="distanceFunc">Function that compute the distance for a given vertex.</param>
        public FibonacciQueue(
            int capacity,
             IEnumerable<TVertex> values,
             Func<TVertex, TDistance> distanceFunc)
            : this(
                capacity,
                values,
                distanceFunc,
                Comparer<TDistance>.Default.Compare
            ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibonacciQueue{TVertex,TDistance}"/> class.
        /// </summary>
        /// <param name="capacity">Initial capacity.</param>
        /// <param name="values">Set of vertices (null if <paramref name="capacity"/> is 0).</param>
        /// <param name="distanceFunc">Function that compute the distance for a given vertex.</param>
        /// <param name="distanceComparison">Comparer of distances.</param>
        public FibonacciQueue(
            int capacity,
             IEnumerable<TVertex> values,
             Func<TVertex, TDistance> distanceFunc,
             Comparison<TDistance> distanceComparison
        ) {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");

            _distanceFunc =
                distanceFunc ??
                    throw new ArgumentNullException(
                        nameof(
                            distanceFunc
                        )
                    );

            _cells =
                new Dictionary<TVertex, FibonacciHeapCell<TDistance, TVertex>>(
                    capacity
                );

            if (values != null) {
                foreach (TVertex vertex in values) {
                    _cells.Add(
                        vertex,
                        new FibonacciHeapCell<TDistance, TVertex> {
                            Priority = _distanceFunc(vertex),
                            Value = vertex,
                            Removed = true
                        }
                    );
                }
            }

            _heap = new FibonacciHeap<TDistance, TVertex>(
                HeapDirection.Increasing,
                distanceComparison
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibonacciQueue{TVertex,TDistance}"/> class.
        /// </summary>
        /// <param name="values">Dictionary of vertices associates to their distance.</param>
        public FibonacciQueue(
             Dictionary<TVertex, TDistance> values)
            : this(
                values,
                Comparer<TDistance>.Default.Compare
            ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FibonacciQueue{TVertex,TDistance}"/> class.
        /// </summary>
        /// <param name="values">Dictionary of vertices associates to their distance.</param>
        /// <param name="distanceComparison">Comparer of distances.</param>
        public FibonacciQueue(
             Dictionary<TVertex, TDistance> values,
             Comparison<TDistance> distanceComparison) {
            
            if (values is null)
                throw new ArgumentNullException(nameof(values));

            if (distanceComparison is null)
                throw new ArgumentNullException(nameof(distanceComparison));

            _distanceFunc = AlgorithmExtensions.GetIndexer(values);
            _cells = new Dictionary<TVertex, FibonacciHeapCell<TDistance, TVertex>>(values.Count);

            foreach (KeyValuePair<TVertex, TDistance> pair in values) {
                _cells.Add(
                    pair.Key,
                    new FibonacciHeapCell<TDistance, TVertex> {
                        Priority = pair.Value,
                        Value = pair.Key,
                        Removed = true
                    }
                );
            }

            _heap = new FibonacciHeap<TDistance, TVertex>(
                HeapDirection.Increasing,
                distanceComparison
            );
        }

        #region IQueue<TVertex>

        /// <inheritdoc />
        public int Count => _heap.Count;

        /// <inheritdoc />
        public bool Contains(TVertex value) {
            return _cells.TryGetValue(
                value,
                out FibonacciHeapCell<TDistance, TVertex> cell
            ) && !cell.Removed;
        }

        /// <inheritdoc />
        public void Enqueue( TVertex value) {
            _cells[value] =
                _heap.Enqueue(_distanceFunc(value), value);
        }

        /// <inheritdoc />
        
        public TVertex Dequeue() {
            FibonacciHeapCell<TDistance, TVertex> cell = _heap.Top;

            if (cell is null)
                throw new InvalidOperationException("Queue is empty.");

            _heap.Dequeue();
            return cell.Value;
        }

        /// <inheritdoc />
        public TVertex Peek() {
            if (_heap.Top is null)
                throw new InvalidOperationException("Queue is empty.");

            return _heap.Top.Value;
        }

        /// <inheritdoc />
        public TVertex[] ToArray() {
            return _heap.Select(entry => entry.Value).ToArray();
        }

        #endregion

        #region IPriorityQueue<TVertex>

        /// <inheritdoc />
        public void Update( TVertex value) {
            _heap.ChangeKey(_cells[value], _distanceFunc(value));
        }

        #endregion
    }
}
