using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QueryService.Models;
using WebAPI.Models.Requests;

namespace AcceptanceTest.Infrastructure
{
    public class WebApiClient
    {
        private readonly HttpClient _client;
        
        public WebApiClient()
        {
            _client = new HttpClient();
        }

        public async Task<(HttpStatusCode, Guid)> SendRequestToSortNumbers(SortNumbersRequest requestContent)
        {
            var message = new HttpRequestMessage()
                .WithUri($"numbers")
                .WithJsonContent(requestContent)
                .WithMethod(HttpMethod.Post);
            message.Headers.Add("Accept", "application/json");

            var response = await _client.SendAsync(message);
            var body = await response.Content.ReadAsStringAsync();
            var id = JsonConvert.DeserializeObject<Guid>(body);
            
            return (response.StatusCode, id);
        }

        public async Task<(HttpStatusCode, SingleNumbersQueryResponse)> GetSortedNumbersResult(Guid id)
        {
            var message = new HttpRequestMessage()
                .WithUri($"numbers/{id}")
                .WithMethod(HttpMethod.Get);
            message.Headers.Add("Accept", "application/json");

            var response = await _client.SendAsync(message);
            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SingleNumbersQueryResponse>(body);
            
            return (response.StatusCode, result);
        }
    }
}