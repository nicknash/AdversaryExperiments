using AdversaryExperiments.Adversaries.Descendants;
using NUnit.Framework;

namespace AdversaryExperiments.Adversaries
{
    [TestFixture]
    public class CachedDAGTests
    {
        [Test]
        public void CountDescendants_TwoVerticesCalledOnParentAfterCallToExistsDirectedPath_ReturnsOne()
        {
            var dag = new CachedDAG(2);
            dag.AddEdge(0, 1);

            dag.ExistsDirectedPath(0, 1);
            int numDescendants = dag.CountDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(1));
        }

        [Test]
        public void CountDescendants_ThreeVerticesCalledOnRootAfterCallToExistsDirectedPath_ReturnsTwo()
        {
            var dag = new CachedDAG(3);
            dag.AddEdge(0, 1);
            dag.AddEdge(0, 2);

            dag.ExistsDirectedPath(0, 1);
            int numDescendants = dag.CountDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(2));
        }

        [Test]
        public void CountDescendants_ThreeVerticesCalledOnLeafAfterCallToExistsDirectedPath_ReturnsZero()
        {
            var dag = new CachedDAG(3);
            dag.AddEdge(0, 1);
            dag.AddEdge(0, 2);

            dag.ExistsDirectedPath(1, 2);
            int numDescendants = dag.CountDescendants(1);

            Assert.That(numDescendants, Is.EqualTo(0));
        }

        [Test]
        public void CountDescendants_DiamondDAGCalledOnApexAfterCallToExistsDirectedPath_ReturnsThree()
        {
            var dag = new CachedDAG(4);
            dag.AddEdge(0, 1); //     0
            dag.AddEdge(0, 2); //    / \
            dag.AddEdge(1, 3); //   1   2
            dag.AddEdge(2, 3); //    \ /
                               //     3
            
            dag.ExistsDirectedPath(0, 1);
            int numDescendants = dag.CountDescendants(0);

            Assert.That(numDescendants, Is.EqualTo(3));
        }
    }
}
