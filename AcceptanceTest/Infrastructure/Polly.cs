using System;
using System.Net;
using Polly;
using Polly.Retry;
using QueryService.Models;

namespace AcceptanceTest.Infrastructure
{
    public static class PollyPolicies
    {
        public static RetryPolicy<(HttpStatusCode, SingleNumbersQueryResponse)> GetSortedNumbersResult()
        {
            return Policy
                .HandleResult<(HttpStatusCode httpStatusCode, SingleNumbersQueryResponse _)>(tuple => 
                    tuple.httpStatusCode != HttpStatusCode.OK)
                .WaitAndRetry(retryCount: 10, _ => TimeSpan.FromSeconds(2));
        }
    }
}