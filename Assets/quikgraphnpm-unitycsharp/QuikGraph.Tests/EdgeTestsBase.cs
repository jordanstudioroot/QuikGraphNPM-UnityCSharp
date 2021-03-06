using JetBrains.Annotations;
using NUnit.Framework;

namespace QuikGraph.Tests.Structures
{
    /// <summary>
    /// Base class for tests relative to edges structures.
    ///</summary>
    internal abstract class EdgeTestsBase
    {
        private static void AssertAreEqual<T>( T left,  T right)
        {
            bool isValueType = typeof(T).IsValueType;
            if (isValueType)
                Assert.AreEqual(left, right);
            else
                Assert.AreSame(left, right);
        }

        protected static void CheckStructEdge<T>( IEdge<T> edge, T source, T target)
        {
            AssertAreEqual(source, edge.Source);
            AssertAreEqual(target, edge.Target);
        }

        protected static void CheckEdge<T>( IEdge<T> edge,  T source,  T target)
        {
            Assert.IsNotNull(edge.Source);
            AssertAreEqual(source, edge.Source);

            Assert.IsNotNull(edge.Target);
            AssertAreEqual(target, edge.Target);
        }

        protected static void CheckTermEdge<T>( ITermEdge<T> edge,  T source,  T target, int sourceTerm, int targetTerm)
        {
            CheckEdge(edge, source, target);
            Assert.AreEqual(sourceTerm, edge.SourceTerminal);
            Assert.AreEqual(targetTerm, edge.TargetTerminal);
        }

        protected static void CheckTaggedEdge<TVertex, TEdge, TTag>( TEdge edge,  TVertex source,  TVertex target,  TTag tag)
            where TEdge : IEdge<TVertex>, ITagged<TTag>
        {
            CheckEdge(edge, source, target);
            AssertAreEqual(tag, edge.Tag);
        }

        protected static void CheckStructTaggedEdge<TVertex, TEdge, TTag>( TEdge edge, TVertex source, TVertex target,  TTag tag)
            where TEdge : IEdge<TVertex>, ITagged<TTag>
        {
            CheckStructEdge(edge, source, target);
            AssertAreEqual(tag, edge.Tag);
        }
    }
}
