using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using QuikGraph.Constants;


namespace QuikGraph
{
    /// <summary>
    /// The default struct based <see cref="IEdge{TVertex}"/> implementation (it is by design an equatable edge).
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>

    [Serializable]
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}")]
    [StructLayout(LayoutKind.Auto)]
    public struct SEdge<TVertex> : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SEdge{TVertex}"/> struct.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        public SEdge( TVertex source,  TVertex target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Source = source;
            Target = target;
        }

        /// <inheritdoc />
        public TVertex Source { get; }

        /// <inheritdoc />
        public TVertex Target { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(EdgeConstants.EdgeFormatString, Source, Target);
        }
    }
}
