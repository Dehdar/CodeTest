using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace AcceptanceTest.Infrastructure
{
    public static class HttpClientExtensions
    {
        public static HttpRequestMessage WithUri(this HttpRequestMessage message, string uri)
        {
            message.RequestUri = new Uri($"{EnvironmentFixture.RestUrl}/{uri}", UriKind.RelativeOrAbsolute);
            return message;
        }
        
        public static HttpRequestMessage WithJsonContent(this HttpRequestMessage message, object content)
        {
            message.Content = new StringContent(
                JsonConvert.SerializeObject(content),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);
            
            return message;
        }

        public static HttpRequestMessage WithMethod(this HttpRequestMessage message, HttpMethod method)
        {
            message.Method = method;
            return message;
        }
    }
}