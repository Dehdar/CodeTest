using System;
using System.Collections.Immutable;
using MediatR;
using Repositories.Models;

namespace QueryService.Queries
{
    public class AllNumbersQuery : IRequest<ImmutableList<(Guid, Status)>>
    {
    }
}