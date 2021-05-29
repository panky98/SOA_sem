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

                await client.ConnectAsync(options);
                Console.WriteLine("STARTED " + client.IsConnected);

            //string clientId = Guid.NewGuid().ToString();
            //string mqttURI = "emqx";
            //int mqttPort = 1883;
            //bool mqttSecure = false;

            //var messageBuilder = new MqttClientOptionsBuilder()
            //    .WithClientId(clientId)
            //    .WithTcpServer(mqttURI, mqttPort)
            //    .WithCleanSession();

            //var options = mqttSecure
            //  ? messageBuilder
            //    .WithTls()
            //    .Build()
            //  : messageBuilder
            //    .Build();

            //var managedOptions = new ManagedMqttClientOptionsBuilder()
            //  .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            //  .WithClientOptions(options)
            //  .Build();

            //client = new MqttFactory().CreateMqttClient();


            //await client.StartAsync(managedOptions);
            //Console.WriteLine("IsConnected: " + client.IsConnected);
        }
    }
}
