using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Repositories.Infrastructure.ValueConverters
{
    public class IntegersToStringValueConverter : ValueConverter<IEnumerable<int>, string>
    {
        public IntegersToStringValueConverter() : base(integers => 
            IntegersToString(integers), (@string => StringToIntegers(@string)))
        {
        }
    
        private static string IntegersToString(IEnumerable<int> value) => 
            JsonSerializer.Serialize(value);

        private static IEnumerable<int> StringToIntegers(string value) => 
            JsonSerializer.Deserialize<IEnumerable<int>>(value);
    }
}