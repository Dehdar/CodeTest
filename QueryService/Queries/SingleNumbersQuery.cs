using System;
using MediatR;
using QueryService.Models;

namespace QueryService.Queries
{
    public class SingleNumbersQuery : IRequest<SingleNumbersQueryResponse>
    {
        public Guid Id { get; init; }
    }
}