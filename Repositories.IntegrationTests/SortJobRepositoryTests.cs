using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using Repositories.IntegrationTests.Infrastructure;
using Repositories.Models;
using Xunit;

namespace Repositories.IntegrationTests
{
    public class SortJobRepositoryTests : IClassFixture<ServiceCollectionFixture>
    {
        private readonly ISortJobRepository _repository;
        private readonly Mock<IDateTimeOffset> _datetimeOffsetMock;

        public SortJobRepositoryTests(ServiceCollectionFixture serviceCollectionFixture)
        {
            _repository = serviceCollectionFixture.GetService<ISortJobRepository>();
            _datetimeOffsetMock = serviceCollectionFixture.DateTimeOffsetMock;
        }

        [Theory, AutoData]
        public async Task Create_Pending_Job_With_Default_Values(
            Guid id,
            DateTimeOffset timestamp)
        {
            // Act
            await _repository.Create(id, timestamp);

            // Assert
            var dto = await _repository.Get(id);
            Assert.True(
                id == dto.Id &&
                timestamp == dto.CreatedTimestamp &&
                Status.Pending == dto.Status &&
                default == dto.CompletedTimestamp &&
                default == dto.SortedNumbers);
        }

        [Theory, AutoData]
        public async Task Update_Job_To_Completed_Status(
            Guid id,
            DateTimeOffset createdTimestamp,
            DateTimeOffset completedTimestamp,
            IEnumerable<int> sortedNumbers)
        {
            // Arrange
            await _repository.Create(id, createdTimestamp);
            _datetimeOffsetMock.Setup(offset => offset.Now).Returns(completedTimestamp);
                
            // Act
            await _repository.Update(id, sortedNumbers);

            // Assert
            var dto = await _repository.Get(id);
            Assert.True(
                id == dto.Id &&
                createdTimestamp == dto.CreatedTimestamp &&
                Status.Completed == dto.Status &&
                completedTimestamp == dto.CompletedTimestamp &&
                sortedNumbers.SequenceEqual(dto.SortedNumbers));
        }

        [Theory, AutoData]
        public async Task Return_Status_Of_All_Jobs(
            Guid firstId,
            Guid secondId,
            IEnumerable<int> sortedNumbers,
            DateTimeOffset timestamp)
        {
            // Arrange
            await _repository.Create(firstId, timestamp);
            await _repository.Update(firstId, sortedNumbers);
            await _repository.Create(secondId, timestamp);

            // Act
            ImmutableList<SortJobDto> jobs = await _repository.All();

            // Assert
            Assert.True(jobs.Single(dto => dto.Id == firstId).Status == Status.Completed);
            Assert.True(jobs.Single(dto => dto.Id == secondId).Status == Status.Pending);
        }
    }
}