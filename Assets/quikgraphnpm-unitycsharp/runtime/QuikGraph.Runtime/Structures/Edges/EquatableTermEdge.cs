using System;
using System.Collections.Generic;
using System.Diagnostics;



namespace QuikGraph
{
    /// <summary>
    /// An <see cref="ITermEdge{TVertex}"/> implementation that supports equality.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>

    [Serializable]
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}")]
    public class EquatableTermEdge<TVertex> : TermEdge<TVertex>, IEquatable<EquatableTermEdge<TVertex>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableTermEdge{TVertex}"/> class
        /// using source/target vertices and zero terminals.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        public EquatableTermEdge( TVertex source,  TVertex target)
            : base(source, target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableTermEdge{TVertex}"/> class
        /// using source/target vertices and source/target terminals.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="sourceTerminal">The source terminal.</param>
        /// <param name="targetTerminal">The target terminal.</param>
        public EquatableTermEdge( TVertex source,  TVertex target, int sourceTerminal, int targetTerminal)
            : base(source, target, sourceTerminal, targetTerminal)
        {
        }

        /// <inheritdoc />
        public virtual bool Equals(EquatableTermEdge<TVertex> other)
        {
            if (other is null)
                return false;
            return EqualityComparer<TVertex>.Default.Equals(Source, other.Source)
                   && EqualityComparer<TVertex>.Default.Equals(Target, other.Target)
                   && SourceTerminal.Equals(other.SourceTerminal)
                   && TargetTerminal.Equals(other.TargetTerminal);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as EquatableTermEdge<TVertex>);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCodeHelpers.Combine(
                Source.GetHashCode(),
                Target.GetHashCode(),
                SourceTerminal.GetHashCode(),
                TargetTerminal.GetHashCode());
        }
    }
}
