using System;
using System.Collections.Generic;
using CommandService.Commands;

namespace WebAPI.Models.Requests
{
    public record SortNumbersRequest
    {
        public IEnumerable<int> Numbers { get; init; }

        protected internal SortNumbersCommand ToCommand() =>
            new()
            {   
                Id = Guid.NewGuid(),
                UnsortedNumbers = Numbers,
                CreatedTimestamp = DateTimeOffset.Now
            };
    }
}