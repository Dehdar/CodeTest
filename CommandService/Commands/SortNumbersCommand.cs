using System;
using System.Collections.Generic;
using MediatR;

namespace CommandService.Commands
{
    public record SortNumbersCommand : INotification
    {
        public Guid Id { get; init; }
        public IEnumerable<int> UnsortedNumbers { get; init; }
        public DateTimeOffset CreatedTimestamp { get; init; }
    }
}