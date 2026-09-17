using NUnit.Framework;
using System;
using OverPower.Domain.Primitives;

namespace OverPower.Tests.Domain
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void SuccessResult_ExposesValueAndSuccessState()
        {
            // Arrange
            string value = "test_success";

            // Act
            Result<string> result = Result<string>.Success(value);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(value));
        }

        [Test]
        public void SuccessResult_AccessingError_ThrowsInvalidOperationException()
        {
            // Arrange
            Result<string> result = Result<string>.Success("test");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => { var err = result.Error; });
        }

        [Test]
        public void FailureResult_ExposesErrorAndFailureState()
        {
            // Arrange
            var error = new DomainError("test_code", "test_message");

            // Act
            Result<string> result = Result<string>.Failure(error);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error.Code, Is.EqualTo("test_code"));
            Assert.That(result.Error.Message, Is.EqualTo("test_message"));
        }

        [Test]
        public void FailureResult_AccessingValue_ThrowsInvalidOperationException()
        {
            // Arrange
            Result<string> result = Result<string>.Failure(new DomainError("code", "msg"));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => { var val = result.Value; });
        }

        [Test]
        public void DefaultResult_IsRecognizedAsUninitializedState()
        {
            // Arrange
            Result<string> defaultResult = default;

            // Assert
            Assert.That(defaultResult.IsSuccess, Is.False);
            Assert.That(defaultResult.IsFailure, Is.True);
            Assert.Throws<InvalidOperationException>(() => { var err = defaultResult.Error; });
            Assert.Throws<InvalidOperationException>(() => { var val = defaultResult.Value; });
        }
    }
}
