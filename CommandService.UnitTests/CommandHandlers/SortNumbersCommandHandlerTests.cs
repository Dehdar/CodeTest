using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using CommandService.CommandHandlers;
using CommandService.Commands;
using Moq;
using Repositories;
using Serilog;
using Shared.TestDependencies;
using Xunit;

namespace CommandService.UnitTests.CommandHandlers
{
    public class SortNumbersCommandHandlerTests
    {
        [Theory, AutoMoqData]
        public async Task Log_When_Command_Is_Consumed(
            [Frozen] Mock<ILogger> loggerMock,
            SortNumbersCommand command,
            CancellationToken cancellationToken,
            SortNumbersCommandHandler sut)
        {
            // Arrange
            var messageTemplate = "Command {@command} consumed";
            
            // Act
            await sut.Handle(command, cancellationToken);
            
            // Assert
            loggerMock.Verify(logger => 
                    logger.Information(
                        It.Is<string>(actualMessageTemplate => messageTemplate == actualMessageTemplate), 
                        It.Is<SortNumbersCommand>(actualCommand => command == actualCommand )),
                Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task Create_Job_For_Sorting(
            [Frozen] Mock<ISortJobRepository> repositoryMock,
            SortNumbersCommand command,
            CancellationToken cancellationToken,
            SortNumbersCommandHandler sut)
        {
            // Act
            await sut.Handle(command, cancellationToken);
            
            // Assert
            repositoryMock.Verify(repository =>
                repository.Create(
                    command.Id, 
                    command.CreatedTimestamp), 
                Times.Once);
        }
        
        [Theory, AutoMoqData]
        public async Task Update_Job_With_Output_From_SortAlgorithm(
            [Frozen] Mock<ISortingAlgorithm> sortingAlgorithmMock,
            [Frozen] Mock<ISortJobRepository> repositoryMock,
            IEnumerable<int> sortedNumbers,
            SortNumbersCommand command,
            CancellationToken cancellationToken,
            SortNumbersCommandHandler sut)
        {
            // Arrange
            sortingAlgorithmMock
                .Setup(algorithm => algorithm
                    .Sort(command.UnsortedNumbers, cancellationToken))
                .ReturnsAsync(sortedNumbers);
            
            // Act
            await sut.Handle(command, cancellationToken);
            
            // Assert
            repositoryMock.Verify(repository =>
                    repository.Update(
                        command.Id, 
                        sortedNumbers), 
                Times.Once);
        }

        [Theory, AutoMoqData]
        public async Task Call_Create_And_Update_In_Correct_Sequence(
            [Frozen] Mock<ISortJobRepository> repositoryMock,
            SortNumbersCommand command,
            CancellationToken cancellationToken,
            SortNumbersCommandHandler sut)
        {
            var callOrder = 0;
            repositoryMock.Setup(repository => repository
                .Create(It.IsAny<Guid>(), It.IsAny<DateTimeOffset>()))
                .Callback(() => 
                    Assert.Equal(expected:0 , callOrder++));
            repositoryMock.Setup(repository => repository
                .Update(It.IsAny<Guid>(), It.IsAny<IEnumerable<int>>()))
                .Callback(() => 
                    Assert.Equal(expected: 1, callOrder++));
            
            // Act
            await sut.Handle(command, cancellationToken);
        }
        
        [Theory, AutoMoqData]
        public async Task Log_When_Command_Is_Processed(
            [Frozen] Mock<ILogger> loggerMock,
            SortNumbersCommand command,
            CancellationToken cancellationToken,
            SortNumbersCommandHandler sut)
        {
            // Arrange
            var messageTemplate = "Command {@command} processed";
            
            // Act
            await sut.Handle(command, cancellationToken);
            
            // Assert
            loggerMock.Verify(logger => 
                    logger.Information(
                        It.Is<string>(actualMessageTemplate => messageTemplate == actualMessageTemplate), 
                        It.Is<SortNumbersCommand>(actualCommand => command == actualCommand )),
                Times.Once);
        }
        
        [Theory, AutoMoqData]
        public async Task Rethrow_And_Log_If_Exception_Is_Thrown(
            [Frozen] Mock<ILogger> loggerMock,
            [Frozen] Mock<ISortingAlgorithm> sortingAlgorithmMock,
            Exception exception,
            SortNumbersCommand command,
            CancellationToken cancellationToken,
            SortNumbersCommandHandler sut)
        {
            // Arrange
            var messageTemplate = "Processing of command {@command} failed";
            sortingAlgorithmMock
                .Setup(algorithm => algorithm.Sort(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            
            // Act
            await Assert.ThrowsAsync<Exception>(() => sut.Handle(command, cancellationToken));
            
            // Assert
            loggerMock.Verify(logger => 
                    logger.Error(
                        It.Is<Exception>(actualException => exception == actualException),
                        It.Is<string>(actualMessageTemplate => messageTemplate == actualMessageTemplate), 
                        It.Is<SortNumbersCommand>(actualCommand => command == actualCommand)),
                Times.Once);
        }
    }
}