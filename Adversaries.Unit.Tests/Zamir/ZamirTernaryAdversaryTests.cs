using NUnit.Framework;

namespace AdversaryExperiments.Adversaries.Zamir
{
    [TestFixture]
    public class ZamirTernaryAdversaryTests
    {
        private ZamirTernaryAdversary _adversary;

        [SetUp]
        public void Setup()
        {
            _adversary = new ZamirTernaryAdversary(10);
        }

        [Test]
        public void Compare_FirstComparisonBothInRoot_ReturnsLess()
        {
            var result = Compare(0, 1);
        
            Assert.That(result, Is.EqualTo(ZamirTernaryAdversary.Less));
        }

        [TestCase(0, 1, ZamirTernaryAdversary.Less)]
        [TestCase(1, 0, ZamirTernaryAdversary.Greater)]
        public void Compare_TwoElementsOfExistingPair_ReturnsExpected(int first, int second, int expected)
        {
            Compare(0, 1);
            
            var result = Compare(first, second);
        
            Assert.That(result, Is.EqualTo(expected));
        }


        [TestCase(0, 2)]
        [TestCase(0, 3)]
        [TestCase(1, 2)]
        [TestCase(1, 3)]
        public void Compare_ElementsOfTwoExistingPairs_ReturnsLess(int first, int second)
        {
            Compare(0, 1);
            Compare(2, 3);

            var result = Compare(first, second);
            
            Assert.That(result, Is.EqualTo(ZamirTernaryAdversary.Less));
        }

        [TestCase(4, 2, ZamirTernaryAdversary.Greater)]
        [TestCase(2, 4, ZamirTernaryAdversary.Less)]
        public void Compare_RootToItsLeftChild_ReturnsExpected(int first, int second, int expected)
        {
            Compare(0, 1);
            Compare(2, 3);

            Compare(1, 3); // This splits the pairs (0, 1), (2, 3) sending 2 to the left node of the root
 
            var result = Compare(first, second); // root element (4) should go to the right child's right child, so should return 'Greater' for Compare(4, 2) 

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(4, 2, ZamirTernaryAdversary.Greater)]
        [TestCase(2, 4, ZamirTernaryAdversary.Less)]
        public void Compare_RootToItsLeftChildRepeatedComparison_ReturnsExpected(int first, int second, int expected)
        {
            Compare(0, 1);
            Compare(2, 3);

            Compare(1, 3); // This splits the pairs (0, 1), (2, 3) sending 2 to the left node of the root
            Compare(first, second); // root element (4) should go to the right child's right child, so should return 'Greater' for Compare(4, 2) 

            var result = Compare(first, second); // This is answered by a traversal

            Assert.That(result, Is.EqualTo(expected));
        }

        // TODONICK: Write tests for 
        // - the other pair-to-pair pair-splitting cases and their implications (as the previous test is)
        // - the same-node singleton-to-pair cases
        // - the pair-ancestor to singleton descendant cases
        // - already defined ordering cases 


        private int Compare(int x, int y) => _adversary.Compare(_adversary.CurrentData[x], _adversary.CurrentData[y]);
    }
}