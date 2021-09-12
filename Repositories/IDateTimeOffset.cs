using System;

namespace Repositories
{
    public interface IDateTimeOffset
    {
        public DateTimeOffset Now { get; }
    }
    
    public class DateTimeOffsetWrapper : IDateTimeOffset
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}