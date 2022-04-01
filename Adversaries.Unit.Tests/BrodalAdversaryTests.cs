using NUnit.Framework;

namespace AdversaryExperiments.Adversaries.Unit.Tests
{
    [TestFixture]
    public class BrodalAdversaryTests
    {
        [Test]
        public void PlaceHolderAddProperTests()
        {
            var adv = new BrodalAdversary(10);
            var f = adv.Compare(adv.CurrentData[0], adv.CurrentData[1]);
            var s = adv.Compare(adv.CurrentData[0], adv.CurrentData[1]);
            var t = adv.Compare(adv.CurrentData[1], adv.CurrentData[0]);

            Assert.That(f, Is.EqualTo(-1));
            Assert.That(s, Is.EqualTo(-1));
            Assert.That(t, Is.EqualTo(1));
        }
    }
}