using System;
using NUnit.Framework;
using OverPower.Domain.Cards;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class CardInstanceIdTests
    {
        [TestCase(1UL)]
        [TestCase(42UL)]
        [TestCase(ulong.MaxValue)]
        public void Constructor_WithPositiveValue_PreservesValue(ulong value)
        {
            Assert.That(new CardInstanceId(value).Value, Is.EqualTo(value));
        }

        [Test]
        public void Constructor_WithZero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CardInstanceId(0));
        }

        [Test]
        public void Equals_WithSameValue_HasValueSemantics()
        {
            var first = new CardInstanceId(42);
            var second = new CardInstanceId(42);

            Assert.That(first.Equals(second), Is.True);
            Assert.That(first.Equals((object)second), Is.True);
            Assert.That(first == second, Is.True);
            Assert.That(first != second, Is.False);
            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        }

        [Test]
        public void Equals_WithDifferentValueOrType_ReturnsFalse()
        {
            var first = new CardInstanceId(42);
            var second = new CardInstanceId(43);

            Assert.That(first.Equals(second), Is.False);
            Assert.That(first == second, Is.False);
            Assert.That(first != second, Is.True);
            Assert.That(first.Equals(null), Is.False);
            Assert.That(first.Equals((object)42UL), Is.False);
            Assert.That(first.Equals(default(CardInstanceId)), Is.False);
        }

        [Test]
        public void ToString_WithValidId_ReturnsNumericValue()
        {
            Assert.That(new CardInstanceId(42).ToString(), Is.EqualTo("42"));
            Assert.That(new CardInstanceId(ulong.MaxValue).ToString(), Is.EqualTo("18446744073709551615"));
        }
    }
}
