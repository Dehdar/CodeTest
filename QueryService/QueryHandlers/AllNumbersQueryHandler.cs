using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QueryService.Queries;
using Repositories;
using Repositories.Models;

namespace QueryService.QueryHandlers
{
    public class AllNumbersQueryHandler : IRequestHandler<AllNumbersQuery, ImmutableList<(Guid, Status)>>
    {
        private readonly ISortJobRepository _repository;

        public AllNumbersQueryHandler(ISortJobRepository repository)
        {
            _repository = repository;
        }

        public async Task<ImmutableList<(Guid, Status)>> Handle(AllNumbersQuery request, CancellationToken cancellationToken)
        {
            return (await _repository.All())
                .Select(result =>
                    (result.Id, result.Status))
                .ToImmutableList();
        }
    }
}