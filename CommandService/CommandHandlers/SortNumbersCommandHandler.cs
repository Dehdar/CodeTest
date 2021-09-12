using System;
using System.Threading;
using System.Threading.Tasks;
using CommandService.Commands;
using MediatR;
using Repositories;
using Serilog;

namespace CommandService.CommandHandlers
{
    public class SortNumbersCommandHandler : INotificationHandler<SortNumbersCommand>
    {
        private readonly ISortJobRepository _repository;
        private readonly ISortingAlgorithm _sortingAlgorithm;
        private readonly ILogger _logger;

        public SortNumbersCommandHandler(ISortJobRepository repository, ISortingAlgorithm sortingAlgorithm, ILogger logger)
        {
            _repository = repository;
            _sortingAlgorithm = sortingAlgorithm;
            _logger = logger;
        }

        public async Task Handle(SortNumbersCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Information("Command {@command} consumed", command);
                
                await _repository.Create(command.Id, command.CreatedTimestamp);
                var sortedNumbers = await _sortingAlgorithm.Sort(command.UnsortedNumbers, cancellationToken);
                await _repository.Update(command.Id, sortedNumbers);
                
                _logger.Information("Command {@command} processed", command);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Processing of command {@command} failed", command);
                throw;
            }
        }
    }
}