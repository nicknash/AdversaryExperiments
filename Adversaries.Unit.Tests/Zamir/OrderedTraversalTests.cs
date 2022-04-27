using System.Linq;
using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace AdversaryExperiments.Adversaries.Zamir
{
    [TestFixture]
    public class OrderedTraversalTests
    {
        [Test]
        public void Traverse_UnitWithAdditionalUnitAtExtremeLeftAndRight_MatchesExpected()
        {
            var x = Node.CreateUnit();
            var extraLeft = x.Left.Left.Left;
            var extraRight = x.Right.Right.Right;
            extraLeft.EnsureInitialized();
            extraRight.EnsureInitialized();

            var expectedNodes = new[]{GetUnitTraversal(extraLeft), new[]{x.Left.Left, x.Left, x.Left.Right, x, x.Right, x.Right.Right}, GetUnitTraversal(extraRight)}.SelectMany(x => x).ToList();
            var nodeToLabel = Enumerable.Range(0, expectedNodes.Count).Select(i => (expectedNodes[i], (char) (i + 'A'))).ToDictionary(n => n.Item1, n => n.Item2);


            var traversal = OrderedTraversal.Traverse(x).ToArray();

            var traversalLabels = new string(traversal.Select(t => nodeToLabel.TryGetValue(t, out var label) ? label : '?').ToArray());
            var expectedLabels = new string(expectedNodes.Select(n => nodeToLabel[n]).ToArray());
            Assert.That(traversalLabels, Is.EqualTo(expectedLabels));
        }

        private IReadOnlyList<Node> GetUnitTraversal(Node root) => new[]{root.Left.Left, root.Left, root.Left.Right, root, root.Right, root.Right.Right};
    }
}