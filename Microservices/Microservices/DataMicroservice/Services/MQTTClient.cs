using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataMicroservice.Services
{
    public class MQTTClient
    {
        public IManagedMqttClient client;
        public MQTTClient()
        {

        }

        public async Task ConnectAsync()
        { 
                string clientId = Guid.NewGuid().ToString();
                string mqttURI = "tcp://emqx";
                int mqttPort = 1883;
                bool mqttSecure = false;

                var messageBuilder = new MqttClientOptionsBuilder()
                    .WithClientId(clientId)
                    .WithTcpServer(mqttURI, mqttPort)
                    .WithCleanSession();

                var options = mqttSecure
                  ? messageBuilder
                    .WithTls()
                    .Build()
                  : messageBuilder
                    .Build();

                var managedOptions = new ManagedMqttClientOptionsBuilder()
                  .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                  .WithClientOptions(options)
                  .Build();

                client = new MqttFactory().CreateManagedMqttClient();

                await client.StartAsync(managedOptions);
        }
    }
}
