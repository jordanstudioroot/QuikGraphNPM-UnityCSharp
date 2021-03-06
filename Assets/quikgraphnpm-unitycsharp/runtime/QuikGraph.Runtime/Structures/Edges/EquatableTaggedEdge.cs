using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuikGraph.Constants;


namespace QuikGraph
{
    /// <summary>
    /// The default implementation of an <see cref="IEdge{TVertex}"/> that supports tagging and is equatable.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TTag">Tag type.</typeparam>

    [Serializable]
    [DebuggerDisplay("{" + nameof(Source) + "}->{" + nameof(Target) + "}:{" + nameof(Tag) + "}")]
    public class EquatableTaggedEdge<TVertex, TTag> : EquatableEdge<TVertex>, ITagged<TTag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquatableTaggedEdge{TVertex,TTag}"/> class.
        /// </summary>
        /// <param name="source">The source vertex.</param>
        /// <param name="target">The target vertex.</param>
        /// <param name="tag">Edge tag.</param>
        public EquatableTaggedEdge( TVertex source,  TVertex target,  TTag tag)
            : base(source, target)
        {
            _tag = tag;
        }

        /// <inheritdoc />
        public event EventHandler TagChanged;

        /// <summary>
        /// Event invoker for <see cref="TagChanged"/> event.
        /// </summary>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnTagChanged( EventArgs args)
        {
            Debug.Assert(args != null);

            TagChanged?.Invoke(this, args);
        }

        private TTag _tag;

        /// <inheritdoc />
        public TTag Tag
        {
            get => _tag;
            set
            {
                if (EqualityComparer<TTag>.Default.Equals(_tag, value))
                    return;

                _tag = value;
                OnTagChanged(EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(EdgeConstants.TaggedEdgeFormatString, Source, Target, Tag?.ToString() ?? "no tag");
        }
    }
}
