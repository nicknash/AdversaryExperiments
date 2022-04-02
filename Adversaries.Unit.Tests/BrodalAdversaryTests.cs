using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AdversaryExperiments.Adversaries.Unit.Tests
{
    [TestFixture]
    public class BrodalAdversaryTests
    {
        private Random _random;
        private BrodalAdversary _adversary;
        private int _numElements;
        
        [SetUp]
        public void Setup()
        {
            _random = new();
            _numElements = _random.Next(10, 1000);
            _adversary = new BrodalAdversary(_numElements);
        }
        
        [Test]
        public void Compare_FirstComparison_AlwaysSaysLess()
        {
            var idx = DontCareIndex();
            var otherIdx = DontCareIndexExcept(idx);
            
            var r1 = _adversary.Compare(_adversary.CurrentData[idx], _adversary.CurrentData[otherIdx]);
            var r2 = _adversary.Compare(_adversary.CurrentData[idx], _adversary.CurrentData[otherIdx]);
            
            Assert.That(r1, Is.EqualTo(-1));
            Assert.That(r1, Is.EqualTo(r2));
        }

        [Test]
        public void Compare_TwoComparisonsSecondComparisonToFirstElementOfFirstComparison_SaysGreater()
        {
            var idx1 = DontCareIndex();
            var idx2 = DontCareIndexExcept(idx1);
            var idx3 = DontCareIndexExcept(idx1, idx2);
            
            var r1 = _adversary.Compare(_adversary.CurrentData[idx1], _adversary.CurrentData[idx2]);
            var r2 = _adversary.Compare(_adversary.CurrentData[idx3], _adversary.CurrentData[idx1]);
            
            Assert.That(r1, Is.EqualTo(-1));
            Assert.That(r2, Is.EqualTo(1));
        }
        
        [Test]
        public void Compare_TwoComparisonsSecondComparisonToSecondElementOfFirstComparison_SaysLess()
        {
            var idx1 = DontCareIndex();
            var idx2 = DontCareIndexExcept(idx1);
            var idx3 = DontCareIndexExcept(idx1, idx2);
            
            var r1 = _adversary.Compare(_adversary.CurrentData[idx1], _adversary.CurrentData[idx2]);
            var r2 = _adversary.Compare(_adversary.CurrentData[idx3], _adversary.CurrentData[idx2]);
            
            Assert.That(r1, Is.EqualTo(-1));
            Assert.That(r2, Is.EqualTo(-1));
        }
        
        // TODONICK: Add a transitivity test.
        
        int DontCareIndex() => _random.Next(0, _numElements);
        
        int DontCareIndexExcept(params int[] except)
        {
            var idx = DontCareIndex();
            while (except.Contains(idx))
            {
                idx = DontCareIndex();
            }
            return idx;
        }
    }
}