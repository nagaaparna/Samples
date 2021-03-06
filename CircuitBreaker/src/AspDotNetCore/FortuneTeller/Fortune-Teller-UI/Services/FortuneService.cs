﻿using Microsoft.Extensions.Logging;
using Pivotal.Discovery.Client;
using Steeltoe.CircuitBreaker.Hystrix;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fortune_Teller_UI.Services
{
    public class FortuneService : HystrixCommand<string>, IFortuneService
    {
        DiscoveryHttpClientHandler _handler;
        ILogger<FortuneService> _logger;
        private const string RANDOM_FORTUNE_URL = "https://fortuneService/api/fortunes/random";


        public FortuneService(IHystrixCommandOptions options, IDiscoveryClient client, ILoggerFactory logFactory) : base(options)
        {
            _handler = new DiscoveryHttpClientHandler(client, logFactory.CreateLogger<DiscoveryHttpClientHandler>());
            // Remove comment to use SSL communications with Self-Signed Certs
            // _handler.ServerCertificateCustomValidationCallback = (a,b,c,d) => {return true;};
            IsFallbackUserDefined = true;
            this._logger = logFactory.CreateLogger<FortuneService>();
        }

        public async Task<string> RandomFortuneAsync()
        {
            var result = await ExecuteAsync();
            return result;
        }

        protected override string Run()
        {
            var client = GetClient();
            var result =  client.GetStringAsync(RANDOM_FORTUNE_URL).Result;
            _logger.LogInformation("Run: {0}", result);
            return result;
        }

        protected override string RunFallback()
        {
            _logger.LogInformation("RunFallback");
            return "{\"id\":1,\"text\":\"You will have a happy day!\"}";
        }

        private HttpClient GetClient()
        {
            var client = new HttpClient(_handler, false);
            return client;
        }
    }
}
