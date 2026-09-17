using System;
using System.Collections.Generic;
using OverPower.Domain.Primitives;

namespace OverPower.Domain.Content
{
    public sealed class ContentIdRegistry
    {
        private readonly HashSet<ContentId> _registeredIds = new HashSet<ContentId>();

        public IReadOnlyCollection<ContentId> RegisteredIds => _registeredIds;

        public Result Register(ContentId contentId)
        {
            if (contentId == null)
            {
                throw new ArgumentNullException(nameof(contentId));
            }

            if (_registeredIds.Contains(contentId))
            {
                return Result.Failure(new DomainError(
                    "content_id.duplicate", 
                    $"Duplicate Content ID registration attempted: '{contentId}' is already registered."
                ));
            }

            _registeredIds.Add(contentId);
            return Result.Success();
        }

        public bool IsRegistered(ContentId contentId)
        {
            if (contentId == null)
            {
                throw new ArgumentNullException(nameof(contentId));
            }
            return _registeredIds.Contains(contentId);
        }

        public void Clear()
        {
            _registeredIds.Clear();
        }
    }
}
