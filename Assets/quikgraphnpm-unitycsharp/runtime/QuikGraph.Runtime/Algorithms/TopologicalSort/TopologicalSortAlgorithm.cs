using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuikGraph.Algorithms.Search;


namespace QuikGraph.Algorithms.TopologicalSort
{
    /// <summary>
    /// Topological sort algorithm (can be performed on an acyclic graph).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [Serializable]
    public sealed class TopologicalSortAlgorithm<TVertex, TEdge>
        : AlgorithmBase<IVertexListGraph<TVertex, TEdge>>
        , IVertexTimeStamperAlgorithm<TVertex>
        where TEdge : IEdge<TVertex>
    {
        
        private readonly IList<TVertex> _sortedVertices;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopologicalSortAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="capacity">Sorted vertices capacity.</param>
        public TopologicalSortAlgorithm(
             IVertexListGraph<TVertex, TEdge> visitedGraph,
            int capacity = -1)
            : base(visitedGraph)
        {
            _sortedVertices = capacity > 0 ? new List<TVertex>(capacity) : new List<TVertex>();
        }

        /// <summary>
        /// Sorted vertices.
        /// </summary>
        /// <remarks>It is null if the algorithm has not been run yet.</remarks>
        
        public TVertex[] SortedVertices { get; private set; }

        private static void OnBackEdge( TEdge edge)
        {
            Debug.Assert(edge != null);

            throw new NonAcyclicGraphException();
        }

        private void OnVertexFinished( TVertex vertex)
        {
            Debug.Assert(vertex != null);

            _sortedVertices.Add(vertex);
        }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            SortedVertices = null;
            _sortedVertices.Clear();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            DepthFirstSearchAlgorithm<TVertex, TEdge> dfs = null;
            try
            {
                dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(
                    this,
                    VisitedGraph,
                    new Dictionary<TVertex, GraphColor>(VisitedGraph.VertexCount));
                dfs.BackEdge += OnBackEdge;
                dfs.FinishVertex += OnVertexFinished;
                dfs.DiscoverVertex += DiscoverVertex;
                dfs.FinishVertex += FinishVertex;

                dfs.Compute();
            }
            finally
            {
                if (dfs != null)
                {
                    dfs.BackEdge -= OnBackEdge;
                    dfs.FinishVertex -= OnVertexFinished;
                    dfs.DiscoverVertex -= DiscoverVertex;
                    dfs.FinishVertex -= FinishVertex;

                    SortedVertices = _sortedVertices.Reverse().ToArray();
                }
            }
        }

        #endregion

        #region IVertexTimeStamperAlgorithm<TVertex>

        /// <inheritdoc />
        public event VertexAction<TVertex> DiscoverVertex;

        /// <inheritdoc />
        public event VertexAction<TVertex> FinishVertex;

        #endregion
    }
}