using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AdversaryExperiments.Adversaries.Unit.Tests
{
    [TestFixture]
    class SimpleDAGTests
    {
        [Test]
        public void ExistsDirectedPath_FromVertexToItself_ReturnsFalse()
        {
            var dag = new SimpleDAG(1);

            Assert.False(dag.ExistsDirectedPath(0, 0));
        }

        [Test]
        public void ExistsDirectedPath_NoEdges_ReturnsFalse()
        {
            var dag = new SimpleDAG(2);

            Assert.False(dag.ExistsDirectedPath(0, 1));
        }

        [Test]
        public void ExistsDirectedPath_SingleEdgeCorrectDirection_ReturnsTrue()
        {
            var dag = new SimpleDAG(2);

            dag.AddEdge(0, 1);

            Assert.True(dag.ExistsDirectedPath(0, 1));
        }

        [Test]
        public void ExistsDirectedPath_SingleEdgeWrongDirection_ReturnsFalse()
        {
            var dag = new SimpleDAG(2);

            dag.AddEdge(0, 1);

            Assert.False(dag.ExistsDirectedPath(1, 0));
        }

        [Test]
        public void ExistsDirectedPath_TwoEdgesCorrectDirection_ReturnsTrue()
        {
            var dag = new SimpleDAG(3);

            dag.AddEdge(0, 1);
            dag.AddEdge(1, 2);

            Assert.True(dag.ExistsDirectedPath(0, 2));
        }

        [Test]
        public void ExistsDirectedPath_ThreeEdgesCorrectDirection_ReturnsTrue()
        {
            var dag = new SimpleDAG(4);

            dag.AddEdge(0, 1);
            dag.AddEdge(1, 2);
            dag.AddEdge(2, 3);

            Assert.True(dag.ExistsDirectedPath(0, 3));
        }

        [Test]
        public void ExistsDirectedPath_ThreeVerticesThreeEdgesNoPath_ReturnsFalse()
        {
            var dag = new SimpleDAG(3);

            dag.AddEdge(0, 1);
            dag.AddEdge(2, 1);

            Assert.False(dag.ExistsDirectedPath(0, 2));
        }

        [Test]
        public void NumDescendants_SingleVertex_ReturnsZero()
        {
            var dag = new SimpleDAG(1);

            int numDescendants = dag.NumDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(0));
        }

        [Test]
        public void NumDescendants_TwoVerticesCalledOnParent_ReturnsOne()
        {
            var dag = new SimpleDAG(2);
            dag.AddEdge(0, 1);

            int numDescendants = dag.NumDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(1));
        }

        [Test]
        public void NumDescendants_ThreeVerticesCalledOnRoot_ReturnsTwo()
        {
            var dag = new SimpleDAG(3);
            dag.AddEdge(0, 1);
            dag.AddEdge(0, 2);

            int numDescendants = dag.NumDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(2));
        }

        [Test]
        public void NumDescendants_ThreeVerticesCalledOnLeaf_ReturnsZero()
        {
            var dag = new SimpleDAG(3);
            dag.AddEdge(0, 1);
            dag.AddEdge(0, 2);

            int numDescendants = dag.NumDescendants(1);

            Assert.That(numDescendants, Is.EqualTo(0));
        }
    }
}
