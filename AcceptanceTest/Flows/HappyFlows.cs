using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AcceptanceTest.Infrastructure;
using Polly.Retry;
using QueryService.Models;
using WebAPI.Models.Requests;
using Xunit;

namespace AcceptanceTest.Flows
{
    public class HappyFlows : IClassFixture<EnvironmentFixture>
    {
        private readonly RetryPolicy<(HttpStatusCode, SingleNumbersQueryResponse)> _retry;
        
        public HappyFlows()
        {
            _retry = PollyPolicies.GetSortedNumbersResult();
        }
        
        [Fact]
        public async Task Request_Sorting_Of_Numbers_And_Get_Result()
        {
            // Arrange
            var request = new SortNumbersRequest { Numbers = new[] { 1, 9, 2, 8, 7, 3, 6, 4, 5 }};
            var client = new WebApiClient();

            // Act 
            var (statusCodeForSendRequest, id) = await client.SendRequestToSortNumbers(request);
            var (statusCodeForGetRequest, result) = _retry.Execute(() => client.GetSortedNumbersResult(id).Result);

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, statusCodeForSendRequest);
            Assert.Equal(HttpStatusCode.OK, statusCodeForGetRequest);
            Assert.True(result.SortedNumbers.SequenceEqual(new []{1, 2, 3, 4, 5, 6, 7, 8, 9}));
        }
    }
}