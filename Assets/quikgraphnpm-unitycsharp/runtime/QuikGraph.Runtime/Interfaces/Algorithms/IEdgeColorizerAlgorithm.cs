using System.Collections.Generic;


namespace QuikGraph.Algorithms
{
    /// <summary>
    /// Represents a storage of edges colorization state.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    public interface IEdgeColorizerAlgorithm<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Treated edges with their colors (colorized edges).
        /// </summary>
        
        IDictionary<TEdge, GraphColor> EdgesColors { get; }
    }
}
