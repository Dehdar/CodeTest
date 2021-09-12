using System;
using System.Collections.Generic;
using Repositories.Models;

namespace QueryService.Models
{
    public record SingleNumbersQueryResponse
    {
        public Guid Id { get; init; }
        public IEnumerable<int> SortedNumbers { get; init; }
        public DateTimeOffset Timestamp { get; init; }
        public TimeSpan Duration { get; init; }
        public Status Status { get; init; }
    }
}