using System;
using System.Collections.Generic;


namespace QuikGraph.Collections {
    /// <summary>
    /// Stores a list of edges.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [Serializable]
    public sealed class EdgeList<TVertex, TEdge> :
        List<TEdge>, IEdgeList<TVertex, TEdge>
        where TEdge : IEdge<TVertex> {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeList{TVertex,TEdge}"/> class.
        /// </summary>
        public EdgeList() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeList{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="capacity">List capacity.</param>
        public EdgeList(int capacity) : base(capacity) { }

        /// <inheritdoc />
        public EdgeList( EdgeList<TVertex, TEdge> other) 
            : base(other) { }

        /// <summary>
        /// Clones this edge list.
        /// </summary>
        /// <returns>Cloned list.</returns>
        
        public EdgeList<TVertex, TEdge> Clone() {
            return new EdgeList<TVertex, TEdge>(this);
        }

        /// <inheritdoc />
        IEdgeList<TVertex, TEdge> IEdgeList<TVertex, TEdge>.Clone() {
            return Clone();
        }

        /// <inheritdoc />
        object ICloneable.Clone() {
            return Clone();
        }
    }
}
