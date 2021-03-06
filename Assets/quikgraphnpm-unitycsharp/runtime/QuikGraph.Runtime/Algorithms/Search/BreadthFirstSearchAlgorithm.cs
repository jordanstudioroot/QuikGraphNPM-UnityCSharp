using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuikGraph.Algorithms.Services;
using QuikGraph.Collections;


namespace QuikGraph.Algorithms.Search
{
    /// <summary>
    /// A breath first search algorithm for directed graphs.
    /// </summary>
    /// <remarks>
    /// This is a modified version of the classic DFS algorithm
    /// where the search is performed both in depth and height.
    /// </remarks>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [Serializable]
    public sealed class BreadthFirstSearchAlgorithm<TVertex, TEdge>
        : RootedAlgorithmBase<TVertex, IVertexListGraph<TVertex, TEdge>>
        , IVertexPredecessorRecorderAlgorithm<TVertex, TEdge>
        , IDistanceRecorderAlgorithm<TVertex>
        , IVertexColorizerAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        
        private readonly IQueue<TVertex> _vertexQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        public BreadthFirstSearchAlgorithm( IVertexListGraph<TVertex, TEdge> visitedGraph)
            : this(visitedGraph, new Collections.Queue<TVertex>(), new Dictionary<TVertex, GraphColor>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexQueue">Queue of vertices to treat.</param>
        /// <param name="verticesColors">Vertices associated to their colors (treatment states).</param>
        public BreadthFirstSearchAlgorithm(
             IVertexListGraph<TVertex, TEdge> visitedGraph,
             IQueue<TVertex> vertexQueue,
             IDictionary<TVertex, GraphColor> verticesColors)
            : this(null, visitedGraph, vertexQueue, verticesColors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexQueue">Queue of vertices to treat.</param>
        /// <param name="verticesColors">Vertices associated to their colors (treatment states).</param>
        public BreadthFirstSearchAlgorithm(
             IAlgorithmComponent host,
             IVertexListGraph<TVertex, TEdge> visitedGraph,
             IQueue<TVertex> vertexQueue,
             IDictionary<TVertex, GraphColor> verticesColors)
            : this(host, visitedGraph, vertexQueue, verticesColors, edges => edges)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BreadthFirstSearchAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="host">Host to use if set, otherwise use this reference.</param>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexQueue">Queue of vertices to treat.</param>
        /// <param name="verticesColors">Vertices associated to their colors (treatment states).</param>
        /// <param name="outEdgesFilter">Function that is used filter out-edges of a vertex.</param>
        public BreadthFirstSearchAlgorithm(
             IAlgorithmComponent host,
             IVertexListGraph<TVertex, TEdge> visitedGraph,
             IQueue<TVertex> vertexQueue,
             IDictionary<TVertex, GraphColor> verticesColors,
             Func<IEnumerable<TEdge>, IEnumerable<TEdge>> outEdgesFilter)
            : base(host, visitedGraph)
        {
            VerticesColors = verticesColors ?? throw new ArgumentNullException(nameof(verticesColors));
            _vertexQueue = vertexQueue ?? throw new ArgumentNullException(nameof(vertexQueue));
            OutEdgesFilter = outEdgesFilter ?? throw new ArgumentNullException(nameof(outEdgesFilter));
        }

        /// <summary>
        /// Filter of edges.
        /// </summary>
        
        public Func<IEnumerable<TEdge>, IEnumerable<TEdge>> OutEdgesFilter { get; }

        #region Events

        /// <inheritdoc />
        public event VertexAction<TVertex> InitializeVertex;

        private void OnVertexInitialized( TVertex vertex)
        {
            Debug.Assert(vertex != null);

            InitializeVertex?.Invoke(vertex);
        }

        /// <inheritdoc />
        public event VertexAction<TVertex> StartVertex;

        private void OnStartVertex( TVertex vertex)
        {
            Debug.Assert(vertex != null);

            StartVertex?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when a vertex is going to be analyzed.
        /// </summary>
        public event VertexAction<TVertex> ExamineVertex;

        private void OnExamineVertex( TVertex vertex)
        {
            Debug.Assert(vertex != null);

            ExamineVertex?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when a vertex is discovered and under treatment.
        /// </summary>
        public event VertexAction<TVertex> DiscoverVertex;

        private void OnDiscoverVertex( TVertex vertex)
        {
            Debug.Assert(vertex != null);

            DiscoverVertex?.Invoke(vertex);
        }

        /// <summary>
        /// Fired when an edge is going to be analyzed.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> ExamineEdge;

        private void OnExamineEdge( TEdge edge)
        {
            Debug.Assert(edge != null);

            ExamineEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a white vertex.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> TreeEdge;

        private void OnTreeEdge( TEdge edge)
        {
            Debug.Assert(edge != null);

            TreeEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when an edge is going to be treated when coming from a gray vertex.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> NonTreeEdge;

        private void OnNonTreeEdge( TEdge edge)
        {
            Debug.Assert(edge != null);

            NonTreeEdge?.Invoke(edge);
        }

        /// <summary>
        /// Fired when the target vertex of an out-edge from the currently treated vertex is marked as gray.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> GrayTarget;

        private void OnGrayTarget( TEdge edge)
        {
            Debug.Assert(edge != null);

            GrayTarget?.Invoke(edge);
        }

        /// <summary>
        /// Fired when the target vertex of an out-edge from the currently treated vertex is marked as black.
        /// </summary>
        public event EdgeAction<TVertex, TEdge> BlackTarget;

        private void OnBlackTarget( TEdge edge)
        {
            Debug.Assert(edge != null);

            BlackTarget?.Invoke(edge);
        }

        /// <inheritdoc />
        public event VertexAction<TVertex> FinishVertex;

        private void OnVertexFinished( TVertex vertex)
        {
            Debug.Assert(vertex != null);

            FinishVertex?.Invoke(vertex);
        }

        #endregion

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            ICancelManager cancelManager = Services.CancelManager;
            if (cancelManager.IsCancelling)
                return;

            // Initialize vertices
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                VerticesColors[vertex] = GraphColor.White;
                OnVertexInitialized(vertex);
            }
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            if (VisitedGraph.VertexCount == 0)
                return;

            if (TryGetRootVertex(out TVertex rootVertex))
            {
                AssertRootInGraph(rootVertex);

                // Enqueue select root only
                EnqueueRoot(rootVertex);
            }
            else
            {
                // Enqueue roots
                foreach (TVertex root in VisitedGraph.Roots())
                    EnqueueRoot(root);
            }

            FlushVisitQueue();
        }

        #endregion

        /// <summary>
        /// Stores vertices associated to their colors (treatment state).
        /// </summary>
        
        public IDictionary<TVertex, GraphColor> VerticesColors { get; }

        #region IVertexColorizerAlgorithm<TVertex>

        /// <inheritdoc />
        public GraphColor GetVertexColor(TVertex vertex)
        {
            if (VerticesColors.TryGetValue(vertex, out GraphColor color))
                return color;
            throw new VertexNotFoundException();
        }

        #endregion

        internal void Visit( TVertex root)
        {
            Debug.Assert(root!= null);

            EnqueueRoot(root);
            FlushVisitQueue();
        }

        private void EnqueueRoot( TVertex vertex)
        {
            OnStartVertex(vertex);

            VerticesColors[vertex] = GraphColor.Gray;

            OnDiscoverVertex(vertex);
            _vertexQueue.Enqueue(vertex);
        }

        private void FlushVisitQueue()
        {
            ICancelManager cancelManager = Services.CancelManager;

            while (_vertexQueue.Count > 0)
            {
                if (cancelManager.IsCancelling)
                    return;

                TVertex u = _vertexQueue.Dequeue();
                OnExamineVertex(u);
                foreach (TEdge edge in OutEdgesFilter(VisitedGraph.OutEdges(u)))
                {
                    TVertex v = edge.Target;
                    OnExamineEdge(edge);

                    GraphColor vColor = VerticesColors[v];
                    if (vColor == GraphColor.White)
                    {
                        OnTreeEdge(edge);
                        VerticesColors[v] = GraphColor.Gray;
                        OnDiscoverVertex(v);
                        _vertexQueue.Enqueue(v);
                    }
                    else
                    {
                        OnNonTreeEdge(edge);
                        if (vColor == GraphColor.Gray)
                            OnGrayTarget(edge);
                        else
                            OnBlackTarget(edge);
                    }
                }

                VerticesColors[u] = GraphColor.Black;
                OnVertexFinished(u);
            }
        }
    }
}
