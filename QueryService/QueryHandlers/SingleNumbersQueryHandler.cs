using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QueryService.Models;
using QueryService.Queries;
using Repositories;
using Repositories.Models;

namespace QueryService.QueryHandlers
{
    public class SingleNumbersQueryHandler : IRequestHandler<SingleNumbersQuery, SingleNumbersQueryResponse>
    {
        private readonly ISortJobRepository _repository;

        public SingleNumbersQueryHandler(ISortJobRepository repository)
        {
            _repository = repository;
        }

        public async Task<SingleNumbersQueryResponse> Handle(SingleNumbersQuery request, CancellationToken cancellationToken)
        {
            var dto = await _repository.Get(request.Id);

            if (dto == default)
                return default;
            
            var responseModel = new SingleNumbersQueryResponse
            {
                Id = dto.Id,
                SortedNumbers = dto.SortedNumbers,
                Timestamp = dto.CreatedTimestamp,
                Status = dto.Status,
                Duration = dto.Status == Status.Completed ? dto.CompletedTimestamp.Subtract(dto.CreatedTimestamp) : default
            };
            
            return responseModel;
        }
    }
}