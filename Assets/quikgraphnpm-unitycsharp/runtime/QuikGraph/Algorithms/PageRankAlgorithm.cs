using System;
using System.Collections.Generic;
using System.Linq;

using QuikGraph.Algorithms.Services;
using QuikGraph.Predicates;

namespace QuikGraph.Algorithms.Ranking
{
    /// <summary>
    /// Algorithm that computes the page rank of a graph.
    /// </summary>
    /// <typeparam name="TVertex">Vertex type.</typeparam>
    /// <typeparam name="TEdge">Edge type.</typeparam>
    [Serializable]
    public sealed class PageRankAlgorithm<TVertex, TEdge> : AlgorithmBase<IBidirectionalGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageRankAlgorithm{TVertex,TEdge}"/> class.
        /// </summary>
        /// <param name="visitedGraph">Graph to visit.</param>
        public PageRankAlgorithm( IBidirectionalGraph<TVertex, TEdge> visitedGraph)
            : base(visitedGraph)
        {
        }

        /// <summary>
        /// Ranks per vertices.
        /// </summary>
        
        public IDictionary<TVertex, double> Ranks { get; private set; } = new Dictionary<TVertex, double>();

        private double _damping = 0.85;

        /// <summary>
        /// Gets or sets the damping rate [0-1].
        /// </summary>
        /// <remarks>By default it uses 0.85 which is the value generally used.</remarks>
        public double Damping
        {
            get => _damping;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Damping)} must be in interval [0-1].");
                _damping = value;
            }
        }

        private double _tolerance = 2 * double.Epsilon;

        /// <summary>
        /// Gets or sets the error tolerance (used to stop the algorithm).
        /// </summary>
        public double Tolerance
        {
            get => _tolerance;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(Tolerance)} must be positive or 0.");
                _tolerance = value;
            }
        }

        private int _maxIterations = 60;

        /// <summary>
        /// Gets or sets the maximum number of iterations.
        /// </summary>
        public int MaxIterations
        {
            get => _maxIterations;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(MaxIterations)} must be positive.");
                _maxIterations = value;
            }
        }

        #region AlgorithmBase<TGraph>

        /// <summary>
        /// Initializes all vertices ranks (1 / VertexCount).
        /// </summary>
        private void InitializeRanks()
        {
            Ranks.Clear();
            double initialRank = 1 / (double)VisitedGraph.VertexCount;
            foreach (TVertex vertex in VisitedGraph.Vertices)
            {
                Ranks.Add(vertex, initialRank);
            }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            InitializeRanks();
        }

        /// <inheritdoc />
        protected override void InternalCompute()
        {
            ICancelManager cancelManager = Services.CancelManager;
            IDictionary<TVertex, double> tempRanks = new Dictionary<TVertex, double>();

            // Create filtered graph
            var filterGraph = new FilteredBidirectionalGraph<TVertex, TEdge, IBidirectionalGraph<TVertex, TEdge>>(
                VisitedGraph,
                new InDictionaryVertexPredicate<TVertex, double>(Ranks).Test,
                edge => true);

            int iteration = 0;
            double error;
            do
            {
                if (cancelManager.IsCancelling)
                    return;

                // Compute page ranks
                error = 0;
                foreach (KeyValuePair<TVertex, double> pair in Ranks)
                {
                    if (cancelManager.IsCancelling)
                        return;

                    TVertex vertex = pair.Key;
                    double rank = pair.Value;

                    // Compute sum of PR(pj)/L(pj)
                    double r = 0;
                    foreach (TEdge edge in filterGraph.InEdges(vertex))
                    {
                        r += Ranks[edge.Source] / filterGraph.OutDegree(edge.Source);
                    }

                    // Add sourceRank and store it
                    double newRank = (1 - Damping) + Damping * r;
                    tempRanks[vertex] = newRank;

                    // Compute deviation
                    error += Math.Abs(newRank - rank);
                }

                // Swap ranks
                IDictionary<TVertex, double> temp = Ranks;
                Ranks = tempRanks;
                tempRanks = temp;

                ++iteration;
            } while (error > Tolerance && iteration < MaxIterations);
        }

        #endregion

        /// <summary>
        /// Gets the sum of all ranks.
        /// </summary>
        /// <returns>Ranks sum.</returns>
        
        public double GetRanksSum()
        {
            return Ranks.Values.Sum();
        }

        /// <summary>
        /// Gets the rank average.
        /// </summary>
        /// <returns>Rank average.</returns>
        
        public double GetRanksMean()
        {
            return GetRanksSum() / Ranks.Count;
        }
    }
}
