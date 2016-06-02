﻿using System;
using System.Collections.Generic;

namespace QuickGraph.Algorithms
{
    public class TransitiveClosureAlgorithm<TVertex, TEdge> : AlgorithmBase<BidirectionalGraph<TVertex, TEdge>> where TEdge : IEdge<TVertex>
    {
        public TransitiveClosureAlgorithm(
            BidirectionalGraph<TVertex, TEdge> visitedGraph,
            Func<TVertex, TVertex, TEdge> createEdge 
            )
            : base(visitedGraph)
        {
            TransitiveClosure = new BidirectionalGraph<TVertex, TEdge>();
            _createEdge = createEdge;
        }
        
        public BidirectionalGraph<TVertex, TEdge> TransitiveClosure { get; private set; } //R# will say you do not need this. AppVeyor wants it.

        private readonly Func<TVertex, TVertex, TEdge> _createEdge;

        protected override void InternalCompute()
        {
            // Clone the visited graph
            TransitiveClosure.AddVerticesAndEdgeRange(VisitedGraph.Edges);

            // Iterate in topo order, track indirect ancestors and remove edges from them to the visited vertex
            var ancestorsOfVertices = new Dictionary<TVertex, HashSet<TVertex>>();
            foreach (var vertexId in VisitedGraph.TopologicalSort())
            {
                //TODO think of some heuristic value here. Like (verticesCount / 2) or (verticesCount / 3)
                var thisVertexPredecessors = new List<TVertex>();
                var thisVertexAncestors = new HashSet<TVertex>();
                ancestorsOfVertices[vertexId] = thisVertexAncestors;

                // Get indirect ancestors
                foreach (var inEdge in VisitedGraph.InEdges(vertexId))
                {
                    var predecessor = inEdge.Source;
                    thisVertexPredecessors.Add(predecessor);

                    // Add all the ancestors of the predeccessors
                    thisVertexAncestors.UnionWith(ancestorsOfVertices[predecessor]);
                }

                // Add indirect edges
                foreach (var indirectAncestor in thisVertexAncestors)
                {
                    if (TransitiveClosure.ContainsEdge(indirectAncestor, vertexId)) continue;
                    TransitiveClosure.AddEdge(_createEdge(indirectAncestor, vertexId));
                }

                // Add predecessors to ancestors list
                thisVertexAncestors.UnionWith(thisVertexPredecessors);
            }
        }
    }
}
