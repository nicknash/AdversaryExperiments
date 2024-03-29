﻿using System;
using System.Collections.Generic;
using AdversaryExperiments.Adversaries.Descendants;
using NUnit.Framework;

namespace AdversaryExperiments.Adversaries
{
    [TestFixture]
    class SharedDAGTests
    {
        public static IEnumerable<Func<int, IDAG>> DAGFactories
        {
            get
            {
                yield return numVerts => new SimpleDAG(numVerts);
                yield return numVerts => new CachedDAG(numVerts);
            }
        }

        [TestCaseSource(nameof(DAGFactories))]
        public void ExistsDirectedPath_FromVertexToItself_ReturnsFalse(Func<int, IDAG> dagFactory)
        {
            IDAG dag = dagFactory(1);

            Assert.False(dag.ExistsDirectedPath(0, 0));
        }

        [TestCaseSource(nameof(DAGFactories))]
        public void ExistsDirectedPath_NoEdges_ReturnsFalse(Func<int, IDAG> dagFactory)
        {
            IDAG dag = dagFactory(2);

            Assert.False(dag.ExistsDirectedPath(0, 1));
        }

        [TestCaseSource(nameof(DAGFactories))]
        public void ExistsDirectedPath_SingleEdgeCorrectDirection_ReturnsTrue(Func<int, IDAG> dagFactory)
        {
            IDAG dag = dagFactory(2);

            dag.AddEdge(0, 1);

            Assert.True(dag.ExistsDirectedPath(0, 1));
        }

        [TestCaseSource(nameof(DAGFactories))]
        public void ExistsDirectedPath_SingleEdgeWrongDirection_ReturnsFalse(Func<int, IDAG> dagFactory)
        {
            IDAG dag = dagFactory(2);

            dag.AddEdge(0, 1);

            Assert.False(dag.ExistsDirectedPath(1, 0));
        }

        [TestCaseSource(nameof(DAGFactories))]
        public void ExistsDirectedPath_TwoEdgesCorrectDirection_ReturnsTrue(Func<int, IDAG> dagFactory)
        {
            IDAG dag = dagFactory(3);

            dag.AddEdge(0, 1);
            dag.AddEdge(1, 2);

            Assert.True(dag.ExistsDirectedPath(0, 2));
        }

        [TestCaseSource(nameof(DAGFactories))]
        public void ExistsDirectedPath_ThreeEdgesCorrectDirection_ReturnsTrue(Func<int, IDAG> dagFactory)
        {
            IDAG dag = dagFactory(4);

            dag.AddEdge(0, 1);
            dag.AddEdge(1, 2);
            dag.AddEdge(2, 3);

            Assert.True(dag.ExistsDirectedPath(0, 3));
        }

        [TestCaseSource(nameof(DAGFactories))]
        public void ExistsDirectedPath_ThreeVerticesThreeEdgesNoPath_ReturnsFalse(Func<int, IDAG> dagFactory)
        {
            IDAG dag = dagFactory(3);

            dag.AddEdge(0, 1);
            dag.AddEdge(2, 1);

            Assert.False(dag.ExistsDirectedPath(0, 2));
        }

    }
}
