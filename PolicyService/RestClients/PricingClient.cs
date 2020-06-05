using Microsoft.Extensions.Configuration;
using Polly;
using PricingService.Api.Commands;
using RestEase;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Steeltoe.Common.Discovery;

namespace PolicyService.RestClients
{
    public interface IPricingClient
    {
        [Post]
        Task<CalculatePriceResult> CalculatePrice([Body] CalculatePriceCommand cmd);
    }

    public class PricingClient : IPricingClient
    {
        private readonly IPricingClient client;

        private static Policy retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(retryCount: 3, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(3));

        private ILogger<PricingClient> _logger;

        public PricingClient(IConfiguration configuration, IDiscoveryClient discoveryClient, ILogger<PricingClient> logger)
        {
            var handler = new DiscoveryHttpClientHandler(discoveryClient);
            _logger = logger;
            var httpClient = new HttpClient(handler, false)
            {
                BaseAddress = new Uri(configuration.GetValue<string>("PricingServiceUri"))
            };
            client = RestClient.For<IPricingClient>(httpClient);
        }

        public Task<CalculatePriceResult> CalculatePrice([Body] CalculatePriceCommand cmd)
        {
            return retryPolicy.ExecuteAsync(async () => await client.CalculatePrice(cmd));
        }
    }
}
