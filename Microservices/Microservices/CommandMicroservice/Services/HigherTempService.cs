using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyticsMicroservice.Services
{
    public class HigherTempService : IHostedService
    {

        private readonly MQTTClient client;
        public HigherTempService(MQTTClient client)
        {
            this.client = client;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("STARTED HOSTED SERVICE");
            this.client.ConnectAsync();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
