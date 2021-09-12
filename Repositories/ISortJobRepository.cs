using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.Infrastructure;
using Repositories.Models;

namespace Repositories
{
    public interface ISortJobRepository
    {
        public Task Create(Guid id, DateTimeOffset createdTimestamp);
        public Task Update(Guid id, IEnumerable<int> sortedNumbers);
        public Task<SortJobDto> Get(Guid id);
        public Task<ImmutableList<SortJobDto>> All();
    }
    
    public class SortJobRepository : ISortJobRepository
    {
        private readonly InMemoryDatabase _inMemoryDatabase;
        private readonly IDateTimeOffset _dateTimeOffset;
        
        public SortJobRepository(InMemoryDatabase inMemoryDatabase, IDateTimeOffset dateTimeOffset)
        {
            _inMemoryDatabase = inMemoryDatabase;
            _dateTimeOffset = dateTimeOffset;
        }  
        
        public async Task Create(Guid id, DateTimeOffset createdTimestamp)  
        {  
            _inMemoryDatabase.SortJobTabel.Add(new()
            {
                Id = id,
                CreatedTimestamp = createdTimestamp,
                Status = Status.Pending
            });
            await _inMemoryDatabase.SaveChangesAsync();
        }

        public async Task Update(Guid id, IEnumerable<int> sortedNumbers)
        {
            var sortJobDto = _inMemoryDatabase
                .SortJobTabel
                .Single(job => job.Id == id);
            
            sortJobDto.CompletedTimestamp = _dateTimeOffset.Now;
            sortJobDto.Status = Status.Completed;
            sortJobDto.SortedNumbers = sortedNumbers;
            
            await _inMemoryDatabase.SaveChangesAsync();
        }

        public async Task<SortJobDto> Get(Guid id)
        {
            return await _inMemoryDatabase.SortJobTabel.FirstOrDefaultAsync(job => job.Id == id);
        } 
        
        public Task<ImmutableList<SortJobDto>> All()  
        {  
            return Task.FromResult(_inMemoryDatabase.SortJobTabel.ToImmutableList());  
        } 
    }
}