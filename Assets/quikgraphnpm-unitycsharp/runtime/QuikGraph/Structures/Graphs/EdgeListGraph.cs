using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuikGraph.Collections;


namespace QuikGraph
{
    /// <summary>
    /// Mutable edge list graph data structure.
    /// </summary>
    /// <remarks>Only mutable by its edges, vertices are not stored but computed on demand.</remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type</typeparam>

    [Serializable]
    [DebuggerDisplay("EdgeCount = {" + nameof(EdgeCount) + "}")]
    public class EdgeListGraph<TVertex, TEdge> :
    IMutableEdgeListGraph<TVertex, TEdge>,
    ICloneable
    where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeListGraph{TVertex,TEdge}"/> class.
        /// </summary>
        public EdgeListGraph()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeListGraph{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="isDirected">Indicates if the graph is directed.</param>
        /// <param name="allowParallelEdges">Indicates if parallel edges are allowed.</param>
        public EdgeListGraph(bool isDirected, bool allowParallelEdges)
        {
            IsDirected = isDirected;
            AllowParallelEdges = allowParallelEdges;
        }

        #region IGraph<TVertex,TEdge>

        /// <inheritdoc />
        public bool IsDirected { get; } = true;

        /// <inheritdoc />
        public bool AllowParallelEdges { get; } = true;

        #endregion

        #region IVertexSet<TVertex>

        /// <inheritdoc />
        public bool IsVerticesEmpty => _edges.Count == 0;

        /// <inheritdoc />
        public int VertexCount => GetVertices().Count;

        /// <inheritdoc />
        public IEnumerable<TVertex> Vertices => GetVertices().AsEnumerable();

        private HashSet<TVertex> GetVertices()
        {
            var vertices = new HashSet<TVertex>();
            foreach (TEdge edge in Edges)
            {
                vertices.Add(edge.Source);
                vertices.Add(edge.Target);
            }

            return vertices;
        }

        /// <inheritdoc />
        public bool ContainsVertex(TVertex vertex)
        {
            if (vertex == null)
                throw new ArgumentNullException(nameof(vertex));

            return Edges.Any(
                edge => EqualityComparer<TVertex>.Default.Equals(edge.Source, vertex) || EqualityComparer<TVertex>.Default.Equals(edge.Target, vertex));
        }

        #endregion

        #region IEdgeSet<TVertex,TEdge>

        
        private readonly EdgeEdgeDictionary<TVertex, TEdge> _edges
            = new EdgeEdgeDictionary<TVertex, TEdge>();

        /// <inheritdoc />
        public bool IsEdgesEmpty => _edges.Count == 0;

        /// <inheritdoc />
        public int EdgeCount => _edges.Count;

        /// <inheritdoc />
        public IEnumerable<TEdge> Edges => _edges.Keys;

        /// <inheritdoc />
        public bool ContainsEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            return _edges.ContainsKey(edge);
        }

        private bool ContainsEdge(TVertex source, TVertex target)
        {
            Debug.Assert(source != null);
            Debug.Assert(target != null);

            return _edges.Keys
                .Any(e => IsDirected
                    ? e.SortedVertexEqualityInternal(source, target)
                    : e.UndirectedVertexEqualityInternal(source, target));
        }

        #endregion

        #region IMutableEdgeListGraph<TVertex,TEdge>

        /// <summary>
        /// Adds <paramref name="edge"/> and its vertices to this graph.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <returns>True if the edge was added, false otherwise.</returns>
        public bool AddVerticesAndEdge( TEdge edge)
        {
            return AddEdge(edge);
        }

        /// <summary>
        /// Adds a set of edges (and it's vertices if necessary).
        /// </summary>
        /// <param name="edges">Edges to add.</param>
        /// <returns>The number of edges added.</returns>
        public int AddVerticesAndEdgeRange( IEnumerable<TEdge> edges)
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            TEdge[] edgesArray = edges.ToArray();
            if (edgesArray.Any(e => e == null))
                throw new ArgumentNullException(nameof(edges), "At least one edge is null.");

            int count = 0;
            foreach (TEdge edge in edgesArray)
            {
                if (AddVerticesAndEdge(edge))
                    ++count;
            }

            return count;
        }

        /// <inheritdoc />
        public bool AddEdge(TEdge edge)
        {
            if (AllowParallelEdges)
            {
                if (_edges.ContainsKey(edge))
                    return false;
            }
            else
            {
                if (ContainsEdge(edge.Source, edge.Target))
                    return false;
            }

            _edges.Add(edge, edge);
            OnEdgeAdded(edge);

            return true;
        }

        /// <inheritdoc />
        public int AddEdgeRange(IEnumerable<TEdge> edges)
        {
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            TEdge[] edgesArray = edges.ToArray();
            if (edgesArray.Any(e => e == null))
                throw new ArgumentNullException(nameof(edges), "At least one edge is null.");

            int count = 0;
            foreach (TEdge edge in edgesArray)
            {
                if (AddEdge(edge))
                    ++count;
            }

            return count;
        }

        /// <inheritdoc />
        public event EdgeAction<TVertex, TEdge> EdgeAdded;

        /// <summary>
        /// Called on each added edge.
        /// </summary>
        /// <param name="edge">Added edge.</param>
        protected virtual void OnEdgeAdded( TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeAdded?.Invoke(edge);
        }

        private bool RemoveEdgeInternal( TEdge edge)
        {
            Debug.Assert(edge != null);

            if (_edges.Remove(edge))
            {
                OnEdgeRemoved(edge);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool RemoveEdge(TEdge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));

            return RemoveEdgeInternal(edge);
        }

        /// <inheritdoc />
        public event EdgeAction<TVertex, TEdge> EdgeRemoved;

        /// <summary>
        /// Called on each removed edge.
        /// </summary>
        /// <param name="edge">Removed edge.</param>
        protected virtual void OnEdgeRemoved( TEdge edge)
        {
            Debug.Assert(edge != null);

            EdgeRemoved?.Invoke(edge);
        }

        /// <inheritdoc />
        public int RemoveEdgeIf(EdgePredicate<TVertex, TEdge> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            TEdge[] edgesToRemove = Edges.Where(edge => predicate(edge)).ToArray();

            foreach (TEdge edge in edgesToRemove)
                RemoveEdgeInternal(edge);
            return edgesToRemove.Length;
        }

        #endregion

        /// <inheritdoc />
        public void Clear()
        {
            EdgeEdgeDictionary<TVertex, TEdge> edges = _edges.Clone();
            _edges.Clear();

            foreach (TEdge edge in edges.Keys)
                OnEdgeRemoved(edge);
        }

        #region ICloneable

        private EdgeListGraph(
            bool isDirected,
            bool allowParallelEdges,
             EdgeEdgeDictionary<TVertex, TEdge> edges)
        {
            Debug.Assert(edges != null);

            IsDirected = isDirected;
            AllowParallelEdges = allowParallelEdges;
            _edges = edges;
        }

        /// <summary>
        /// Clones this graph.
        /// </summary>
        /// <returns>Cloned graph.</returns>
        
        
        public EdgeListGraph<TVertex, TEdge> Clone()
        {
            return new EdgeListGraph<TVertex, TEdge>(
                IsDirected,
                AllowParallelEdges,
                _edges.Clone());
        }

        /// <inheritdoc />
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}
