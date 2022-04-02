using AdversaryExperiments.Adversaries.Descendants;
using NUnit.Framework;

namespace AdversaryExperiments.Adversaries
{
    [TestFixture]
    class DescendantsAdversaryTests
    {
        [Test]
        public void Compare_CalledThreeTimes_NumComparisonsIsThree()
        {
            var dagAdv = new DescendantsAdversary(1);
            var data = dagAdv.CurrentData;

            dagAdv.Compare(data[0], data[0]);
            dagAdv.Compare(data[0], data[0]);
            dagAdv.Compare(data[0], data[0]);

            Assert.That(dagAdv.NumComparisons, Is.EqualTo(3));           
        }
        
        [Test]
        public void Compare_OperandWithItself_ComparesEqual()
        {
            var dagAdv = new DescendantsAdversary(1);
            var data = dagAdv.CurrentData;

            int result = dagAdv.Compare(data[0], data[0]);

            Assert.That(result, Is.EqualTo(0));
        }
        
        [Test]
        public void Compare_SameOperandsTwice_SameResult()
        {
            var dagAdv = new DescendantsAdversary(2);
            var data = dagAdv.CurrentData;

            int firstResult = dagAdv.Compare(data[0], data[1]);
            int secondResult = dagAdv.Compare(data[0], data[1]);

            Assert.That(firstResult, Is.EqualTo(secondResult));
        }

        [Test]
        public void Compare_NotComparedBefore_CompareLow()
        {
            var dagAdv = new DescendantsAdversary(2);
            var data = dagAdv.CurrentData;

            int result = dagAdv.Compare(data[0], data[1]);

            Assert.That(result, Is.LessThan(0));        
        }

        [Test]
        public void Compare_SameOperandOppositeWays_OppositeResults()
        {
            var dagAdv = new DescendantsAdversary(2);
            var data = dagAdv.CurrentData;

            int firstToSecond = dagAdv.Compare(data[0], data[1]);
            int secondToFirst = dagAdv.Compare(data[1], data[0]);

            Assert.That(firstToSecond, Is.EqualTo(-secondToFirst));
        }

        [Test]
        public void Compare_ThreeOperandsFirstLessThanSecondAndSecondLessThanThird_IsTransitive()
        {
            var dag = new SimpleDAG(3);
            dag.AddEdge(0, 1);
            dag.AddEdge(1, 2);
            var dagAdv = new DescendantsAdversary(dag);
            var data = dagAdv.CurrentData;

            int thirdToFirst = dagAdv.Compare(data[2], data[0]);

            Assert.That(thirdToFirst, Is.GreaterThan(0));
        }

        [Test]
        public void Compare_LeftOperandHasOneDescendantRightOperandHasNoDescendants_ComparesLow()
        {
            var dag = new SimpleDAG(3);
            dag.AddEdge(0, 1);
            var dagAdv = new DescendantsAdversary(dag);
            var data = dagAdv.CurrentData;

            int result = dagAdv.Compare(data[0], data[2]);

            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void Compare_LeftOperandHasNoDescendantsRightOperandHasOneDescendant_ComparesHigh()
        {
            var dag = new SimpleDAG(3);
            dag.AddEdge(0, 1);
            var dagAdv = new DescendantsAdversary(dag);
            var data = dagAdv.CurrentData;

            int result = dagAdv.Compare(data[2], data[0]);

            Assert.That(result, Is.GreaterThan(0));
        }

    }
}
