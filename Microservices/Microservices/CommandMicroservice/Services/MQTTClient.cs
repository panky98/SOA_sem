using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsMicroservice.Services
{
    public class MQTTClient
    {
        public IMqttClient client;

        public MQTTClient()
        {

        }

        public async Task ConnectAsync()
        {
                var options = new MqttClientOptions() { ClientId = Guid.NewGuid().ToString() };
                options.ChannelOptions = new MqttClientTcpOptions()
                {
                    Server = "emqx",
                    Port = 1883
                };

                options.CleanSession = true;
                options.KeepAlivePeriod = TimeSpan.FromSeconds(100.5);

                client = new MqttFactory().CreateMqttClient();

                client.UseApplicationMessageReceivedHandler((args) =>
                {
                     Console.WriteLine("RECEIVED MESSAGE: " + Encoding.UTF8.GetString(args.ApplicationMessage.Payload) + "From topic: "+args.ApplicationMessage.Topic);

                });

                client.UseConnectedHandler(async (a) =>
                {
                    Console.WriteLine("### CONNECTED WITH SERVER ###");

                    // Subscribe to a topic
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("HigherTemp").Build());

                    Console.WriteLine("### SUBSCRIBED ###");
                });

            await client.ConnectAsync(options);
                Console.WriteLine("STARTED " + client.IsConnected);
        }
    }
}
