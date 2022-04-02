using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AdversaryExperiments.Adversaries;

namespace Adversaries.Unit.Tests
{
    [TestFixture]
    public class McIlroyKillerTests
    {
        [Test]
        public void Compare_CalledThreeTimes_NumComparisonsIsThree()
        {
            var killer = new McIlroyKiller(1);
            var data = killer.CurrentData;

            killer.Compare(data[0], data[0]);
            killer.Compare(data[0], data[0]);
            killer.Compare(data[0], data[0]);

            Assert.That(killer.NumComparisons, Is.EqualTo(3));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void CurrentData_Called_HasCorrectLength(int length)
        {
            var killer = new McIlroyKiller(length);

            var result = killer.CurrentData;

            Assert.That(result.Count, Is.EqualTo(length));
        }

        [Test]
        public void Compare_GasToGasNoCandidatePivot_ComparesHigh()
        {
            var killer = new McIlroyKiller(2);
            var data = killer.CurrentData;

            int result = killer.Compare(data[0], data[1]);

            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void Compare_GasToGasLeftOperandIsCandidatePivot_ComparesLow()
        {
            var killer = new McIlroyKiller(3);
            var data = killer.CurrentData;
            killer.Compare(data[0], data[1]);

            int result = killer.Compare(data[0], data[2]);

            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void Compare_SolidToGas_ComparesLow()
        {
            var killer = new McIlroyKiller(2);
            var data = killer.CurrentData;
            killer.Compare(data[0], data[1]); // data[1] is now solid

            int result = killer.Compare(data[1], data[0]);

            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void Compare_GasToSolid_ComparesHigh()
        {
            var killer = new McIlroyKiller(2);
            var data = killer.CurrentData;
            killer.Compare(data[0], data[1]); // data[1] is now solid

            int result = killer.Compare(data[0], data[1]);

            Assert.That(result, Is.GreaterThan(0));
        }

        [Test]
        public void Compare_SolidToSolidLeftOperandFrozenFirst_ComparesLow()
        {
            var killer = new McIlroyKiller(4);
            var data = killer.CurrentData;
            killer.Compare(data[1], data[0]); // data[0] is now solid
            killer.Compare(data[3], data[2]); // data[2] is now solid

            int result = killer.Compare(data[0], data[2]);

            Assert.That(result, Is.LessThan(0));
        }

        [Test]
        public void Compare_SolidToSolidRightOperandFrozenFirst_ComparesHigh()
        {
            var killer = new McIlroyKiller(4);
            var data = killer.CurrentData;
            killer.Compare(data[3], data[2]); // data[2] is now solid
            killer.Compare(data[1], data[0]); // data[0] is now solid            

            int result = killer.Compare(data[0], data[2]);

            Assert.That(result, Is.GreaterThan(0));
        }
    }
}
