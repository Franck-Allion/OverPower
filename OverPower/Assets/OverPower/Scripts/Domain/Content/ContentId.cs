using System;
using OverPower.Domain.Primitives;

namespace OverPower.Domain.Content
{
    public sealed class ContentId : IEquatable<ContentId>, IComparable<ContentId>
    {
        public string Value { get; }

        private ContentId(string value)
        {
            Value = value;
        }

        public static Result<ContentId> Create(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<ContentId>.Failure(new DomainError("content_id.empty", "Content ID cannot be empty or whitespace."));
            }

            string[] segments = id.Split('.');
            if (segments.Length < 2)
            {
                return Result<ContentId>.Failure(new DomainError("content_id.invalid_format", $"Content ID '{id}' must contain at least two segments separated by a dot."));
            }

            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (string.IsNullOrEmpty(segment))
                {
                    return Result<ContentId>.Failure(new DomainError("content_id.invalid_format", $"Content ID '{id}' contains an empty segment."));
                }

                for (int j = 0; j < segment.Length; j++)
                {
                    char c = segment[j];
                    if (!(c >= 'a' && c <= 'z') && !(c >= '0' && c <= '9') && c != '_')
                    {
                        return Result<ContentId>.Failure(new DomainError("content_id.invalid_format", 
                            $"Content ID '{id}' is invalid. Segment '{segment}' must contain only lowercase ASCII letters, digits, or underscores."));
                    }
                }
            }

            return Result<ContentId>.Success(new ContentId(id));
        }

        public bool Equals(ContentId? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ContentId);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString() => Value;

        public static bool operator ==(ContentId? left, ContentId? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (ReferenceEquals(left, null)) return false;
            return left.Equals(right);
        }

        public static bool operator !=(ContentId? left, ContentId? right)
        {
            return !(left == right);
        }

        public int CompareTo(ContentId? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Value, other.Value, StringComparison.Ordinal);
        }
    }
}
