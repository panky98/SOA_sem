using DataMicroservice.Models;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataMicroservice.Services
{
    public class MQTTClient
    {
        public IManagedMqttClient client;
        private readonly IHttpClientFactory _httpFactory;
        private HttpClient httpClient;

        public MQTTClient(IHttpClientFactory _httpFactory)
        {
            this._httpFactory = _httpFactory;
            this.httpClient = _httpFactory.CreateClient();
        }

        public async Task ConnectAsync()
        {
            if (client == null || (!client.IsStarted))
            {
                string clientId = Guid.NewGuid().ToString();
                string mqttURI = "emqx";
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
                Console.WriteLine("IsConnected: " + client.IsConnected);

                //kreiranje stream-a
                StreamCreation obj = new StreamCreation("create stream demo (Co float, Humidity float,Light boolean,Lpg float, Motion boolean, Smoke float,Temp float, Sensor string) " +
                                                                            "WITH (FORMAT=\"JSON\", DATASOURCE=\"devices/+/messages\")");

                var sendingItem = new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
                await this.httpClient.PostAsync("http://kuiper:9081/streams", sendingItem);

                sendingItem=new StringContent("{ \"id\": \"higher_temp\",\"sql\": \"SELECT * FROM demo;\",\"actions\": [{\"mqtt\": {\"server\": \"tcp://emqx:1883\",\"topic\": \"HigherTemp\"}}, {\"log\":{ }} ] }", Encoding.UTF8, "application/json");
                await this.httpClient.PostAsync("http://kuiper:9081/rules", sendingItem);
            }
        }
    }
}
