using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace QuikGraph.Collections {
    /// <summary>
    /// Priority queue to sort vertices by distance priority (use <see cref="BinaryHeap{TPriority,TValue}"/>).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TDistance">Distance type.</typeparam>
    [Serializable]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public sealed class BinaryQueue<TVertex, TDistance> :
        IPriorityQueue<TVertex> {
        
        
        private readonly Func<TVertex, TDistance> _distanceFunc;

        
        private readonly BinaryHeap<TDistance, TVertex> _heap;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryQueue{TVertex,TDistance}"/> class.
        /// </summary>
        /// <param name="distanceFunc">Function that compute the distance for a given vertex.</param>
        public BinaryQueue( Func<TVertex, TDistance> distanceFunc)
            : this(distanceFunc, Comparer<TDistance>.Default.Compare)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryQueue{TVertex,TDistance}"/> class.
        /// </summary>
        /// <param name="distanceFunc">Function that compute the distance for a given vertex.</param>
        /// <param name="distanceComparison">Comparer of distances.</param>
        public BinaryQueue(
             Func<TVertex, TDistance> distanceFunc, 
             Comparison<TDistance> distanceComparison)
        {
            if (distanceComparison is null)
                throw new ArgumentNullException(nameof(distanceComparison));

            _distanceFunc = distanceFunc ?? throw new ArgumentNullException(nameof(distanceFunc));
            _heap = new BinaryHeap<TDistance, TVertex>(distanceComparison);
        }

        #region IQueue<TVertex>

        /// <inheritdoc />
        public int Count => _heap.Count;

        /// <inheritdoc />
        public bool Contains(TVertex value)
        {
            return _heap.IndexOf(value) > -1;
        }

        /// <inheritdoc />
        public void Enqueue( TVertex value)
        {
            _heap.Add(_distanceFunc(value), value);
        }

        /// <inheritdoc />
        
        public TVertex Dequeue()
        {
            return _heap.RemoveMinimum().Value;
        }

        /// <inheritdoc />
        
        public TVertex Peek()
        {
            return _heap.Minimum().Value;
        }

        /// <inheritdoc />
        public TVertex[] ToArray()
        {
            return _heap.ToArray();
        }

        #endregion

        #region IPriorityQueue

        /// <inheritdoc />
        public void Update( TVertex value)
        {
            _heap.Update(_distanceFunc(value), value);
        }

        #endregion

        /// <summary>
        /// Converts this queue to an array of vertices associated to their distances.
        /// </summary>
        /// <returns>Array composed of elements.</returns>
        
        
        public KeyValuePair<TDistance, TVertex>[] ToPairsArray()
        {
            return _heap.ToPairsArray();
        }

        /// <summary>
        /// Gets an alternative string representation.
        /// </summary>
        /// <returns>String representation.</returns>
        
        
        public string ToString2()
        {
            return _heap.ToString2();
        }
    }
}
