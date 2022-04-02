using AdversaryExperiments.Adversaries.Descendants;
using NUnit.Framework;

namespace AdversaryExperiments.Adversaries
{
    [TestFixture]
    class SimpleDAGTests
    {
        [Test]
        public void CountDescendants_SingleVertex_ReturnsZero()
        {
            var dag = new SimpleDAG(1);

            int numDescendants = dag.CountDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(0));
        }

        [Test]
        public void CountDescendants_TwoVerticesCalledOnParent_ReturnsOne()
        {
            var dag = new SimpleDAG(2);
            dag.AddEdge(0, 1);

            int numDescendants = dag.CountDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(1));
        }

        [Test]
        public void CountDescendants_ThreeVerticesCalledOnRoot_ReturnsTwo()
        {
            var dag = new SimpleDAG(3);
            dag.AddEdge(0, 1);
            dag.AddEdge(0, 2);

            int numDescendants = dag.CountDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(2));
        }

        [Test]
        public void CountDescendants_ThreeVerticesCalledOnLeaf_ReturnsZero()
        {
            var dag = new SimpleDAG(3);
            dag.AddEdge(0, 1);
            dag.AddEdge(0, 2);

            int numDescendants = dag.CountDescendants(1);

            Assert.That(numDescendants, Is.EqualTo(0));
        }

        [Test]
        public void CountDescendants_DiamondDAGCalledOnApex_ReturnsThree()
        {
            var dag = new SimpleDAG(4);
            dag.AddEdge(0, 1); //     0
            dag.AddEdge(0, 2); //    / \
            dag.AddEdge(1, 3); //   1   2
            dag.AddEdge(2, 3); //    \ /
                               //     3

            int numDescendants = dag.CountDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(3));
        }
    }
}
