using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Services;
using QuikGraph.Serialization.DirectedGraphML;


namespace QuikGraph.Serialization
{
    /// <summary>
    /// Algorithm that creates a <see cref="DirectedGraphML.DirectedGraph"/>
    /// from a given directed graph.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public sealed class DirectedGraphMLAlgorithm<TVertex, TEdge> : AlgorithmBase<IVertexAndEdgeListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        
        private readonly VertexIdentity<TVertex> _vertexIdentities;

        
        private readonly EdgeIdentity<TVertex, TEdge> _edgeIdentities;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectedGraphMLAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        /// <param name="vertexIdentities">Vertex identity method.</param>
        /// <param name="edgeIdentities">Edge identity method.</param>
        public DirectedGraphMLAlgorithm(
             IVertexAndEdgeListGraph<TVertex, TEdge> visitedGraph,
             VertexIdentity<TVertex> vertexIdentities,
             EdgeIdentity<TVertex, TEdge> edgeIdentities)
            : base(visitedGraph)
        {
            _vertexIdentities = vertexIdentities ?? throw new ArgumentNullException(nameof(vertexIdentities));
            _edgeIdentities = edgeIdentities ?? throw new ArgumentNullException(nameof(edgeIdentities));
        }

        /// <summary>
        /// Gets the resulting <see cref="DirectedGraphML.DirectedGraph"/>.
        /// </summary>
        public DirectedGraph DirectedGraph { get; private set; }

        #region AlgorithmBase<TGraph>

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            ICancelManager cancelManager = Services.CancelManager;
            DirectedGraph = new DirectedGraph();

            var nodes = new List<DirectedGraphNode>(VisitedGraph.VertexCount);
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                if (cancelManager.IsCancelling)
                    return;

                var node = new DirectedGraphNode { Id = _vertexIdentities(vertex) };
                OnFormatNode(vertex, node);
                nodes.Add(node);
            }

            DirectedGraph.Nodes = nodes.ToArray();

            var links = new List<DirectedGraphLink>(VisitedGraph.EdgeCount);
            foreach (TEdge edge in VisitedGraph.Edges)
            {
                if (cancelManager.IsCancelling)
                    return;

                var link = new DirectedGraphLink
                {
                    Label = _edgeIdentities(edge),
                    Source = _vertexIdentities(edge.Source),
                    Target = _vertexIdentities(edge.Target)
                };
                OnFormatEdge(edge, link);
                links.Add(link);
            }

            DirectedGraph.Links = links.ToArray();

            OnFormatGraph();
        }

        #endregion

        /// <summary>
        /// Fired when a new node is added to the <see cref="DirectedGraph"/>.
        /// </summary>
        public event Action<TVertex, DirectedGraphNode> FormatNode;

        private void OnFormatNode( TVertex vertex,  DirectedGraphNode node)
        {
            Debug.Assert(vertex != null);
            Debug.Assert(node != null);

            FormatNode?.Invoke(vertex, node);
        }

        /// <summary>
        /// Fired when a new link is added to the <see cref="DirectedGraph"/>.
        /// </summary>
        public event Action<TEdge, DirectedGraphLink> FormatEdge;

        private void OnFormatEdge( TEdge edge,  DirectedGraphLink link)
        {
            Debug.Assert(edge != null);
            Debug.Assert(link != null);

            FormatEdge?.Invoke(edge, link);
        }

        /// <summary>
        /// Fired when the graph is about to be returned.
        /// </summary>
        public event Action<IVertexAndEdgeListGraph<TVertex, TEdge>, DirectedGraph> FormatGraph;

        private void OnFormatGraph()
        {
            FormatGraph?.Invoke(VisitedGraph, DirectedGraph);
        }
    }
}
