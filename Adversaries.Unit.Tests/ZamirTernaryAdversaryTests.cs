using AdversaryExperiments.Adversaries.Zamir;
using NUnit.Framework;

namespace AdversaryExperiments.Adversaries
{
    [TestFixture]
    public class ZamirTernaryAdversaryTests
    {

        [Test]
        public void Compare_FirstComparisonBothInRoot_ReturnsLess()
        {
            var adversary = new ZamirTernaryAdversary(10);

            var result = adversary.Compare(adversary.CurrentData[0], adversary.CurrentData[1]);
        
            Assert.That(result, Is.EqualTo(-1));
        }

        public void Compare_RootToRightTreeNode_ReturnsLess()
        {
            // All tests here will need the same-node case dealt with first
        }
    }
}