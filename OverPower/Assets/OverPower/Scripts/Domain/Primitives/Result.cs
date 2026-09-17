using System;

namespace OverPower.Domain.Primitives
{
    public readonly struct Result
    {
        private readonly DomainError _error;
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public DomainError Error
        {
            get
            {
                if (IsSuccess)
                {
                    throw new InvalidOperationException("Cannot access the error of a successful Result.");
                }
                if (_error.Code == null)
                {
                    throw new InvalidOperationException("The Result is in an uninitialized default state.");
                }
                return _error;
            }
        }

        private Result(bool isSuccess, DomainError error)
        {
            IsSuccess = isSuccess;
            _error = error;
        }

        public static Result Success() => new Result(true, default);
        public static Result Failure(DomainError error) => new Result(false, error);
    }

    public readonly struct Result<TValue>
    {
        private readonly TValue _value;
        private readonly DomainError _error;
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public TValue Value
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException("Cannot access the value of a failed Result.");
                }
                return _value;
            }
        }

        public DomainError Error
        {
            get
            {
                if (IsSuccess)
                {
                    throw new InvalidOperationException("Cannot access the error of a successful Result.");
                }
                if (_error.Code == null)
                {
                    throw new InvalidOperationException("The Result is in an uninitialized default state.");
                }
                return _error;
            }
        }

        private Result(TValue value)
        {
            IsSuccess = true;
            _value = value;
            _error = default;
        }

        private Result(DomainError error)
        {
            IsSuccess = false;
            _value = default!;
            _error = error;
        }

        public static Result<TValue> Success(TValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Success value cannot be null.");
            }
            return new Result<TValue>(value);
        }

        public static Result<TValue> Failure(DomainError error) => new Result<TValue>(error);

        public static implicit operator Result<TValue>(TValue value) => Success(value);
        public static implicit operator Result<TValue>(DomainError error) => Failure(error);
    }
}
