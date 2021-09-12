using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Models
{
    public record SortJobDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public Guid Id { get; init; }
        public IEnumerable<int> SortedNumbers { get; set; }
        public DateTimeOffset CreatedTimestamp { get; init; }
        public DateTimeOffset CompletedTimestamp { get; set; }
        public Status Status { get; set; }
   }
    
    public enum Status {Pending, Completed}
}