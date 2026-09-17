using NUnit.Framework;
using OverPower.Domain.Run;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class RunSeedTests
    {
        [Test]
        public void RunSeed_ValueEquality_BehavesDeterministically()
        {
            // Arrange
            var seed1 = new RunSeed(42UL);
            var seed2 = new RunSeed(42UL);
            var seed3 = new RunSeed(99UL);

            // Act & Assert
            Assert.That(seed1.Equals(seed2), Is.True);
            Assert.That(seed1 == seed2, Is.True);
            Assert.That(seed1 != seed2, Is.False);
            Assert.That(seed1.GetHashCode(), Is.EqualTo(seed2.GetHashCode()));

            Assert.That(seed1.Equals(seed3), Is.False);
            Assert.That(seed1 == seed3, Is.False);
            Assert.That(seed1 != seed3, Is.True);
            Assert.That(seed1.GetHashCode(), Is.Not.EqualTo(seed3.GetHashCode()));

            Assert.That(seed1.ToString(), Is.EqualTo("42"));
        }
    }
}
