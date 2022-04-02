using System;
using NUnit.Framework;

namespace AdversaryExperiments.Adversaries.Unit.Tests
{
    [TestFixture]
    public class BrodalAdversaryTests
    {
        private Random _random = new();
        
        [Test]
        public void Compare_FirstComparison_AlwaysSaysLess()
        {
            var n = DontCare(100);
            var adv = new BrodalAdversary(n);
            var idx = DontCare(n);
            var otherIdx = (idx + 1 + DontCare(n - 1)) % n;
            
            var r1 = adv.Compare(adv.CurrentData[idx], adv.CurrentData[otherIdx]);
            var r2 = adv.Compare(adv.CurrentData[idx], adv.CurrentData[otherIdx]);
            
            Assert.That(r1, Is.EqualTo(-1));
            Assert.That(r1, Is.EqualTo(r2));
        }
        
        

        int DontCare(int max) => _random.Next(0, max);
    }
}