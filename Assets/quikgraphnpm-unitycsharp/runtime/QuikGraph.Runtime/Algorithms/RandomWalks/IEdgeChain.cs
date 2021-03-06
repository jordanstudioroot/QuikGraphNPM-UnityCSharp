using System.Collections.Generic;


namespace QuikGraph.Algorithms.RandomWalks
{
    /// <summary>
    /// Set of edges forming chain of edges.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IEdgeChain<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Tries to get the successor of the given <paramref name="vertex"/> in the given <paramref name="graph"/>.
        /// </summary>
        /// <param name="graph">The graph to search in.</param>
        /// <param name="vertex">The vertex.</param>
        /// <param name="successor">Found successor, otherwise null.</param>
        /// <returns>True if a successor was found, false otherwise.</returns>
        
        bool TryGetSuccessor( IImplicitGraph<TVertex, TEdge> graph,  TVertex vertex, out TEdge successor);

        /// <summary>
        /// Tries to get the successor of the given <paramref name="vertex"/> in the given set of <paramref name="edges"/>.
        /// </summary>
        /// <param name="edges">Edge set in which searching.</param>
        /// <param name="vertex">The vertex.</param>
        /// <param name="successor">Found successor, otherwise null.</param>
        /// <returns>True if a successor was found, false otherwise.</returns>
        
        bool TryGetSuccessor( IEnumerable<TEdge> edges,  TVertex vertex, out TEdge successor);
    }
}
