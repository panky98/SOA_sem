using Microsoft.AspNetCore.SignalR.Client;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnalyticsMicroservice.Services
{
    public class MQTTClient
    {
        public IMqttClient client;
        private readonly IHttpClientFactory _httpFactory;
        private HttpClient httpClient;
        readonly RedisClient redis = new RedisClient("redis-api-events", 6379);
        HubConnection connection;


        public MQTTClient(IHttpClientFactory _httpFactory)
        {
            this._httpFactory = _httpFactory;
            this.httpClient = _httpFactory.CreateClient();
            connection = new HubConnectionBuilder()
            .WithUrl("http://datamicroservice:80/eventhub")
            .Build();
        }

        public async Task ConnectAsync()
        {
            await connection.StartAsync();
                var options = new MqttClientOptions() { ClientId = Guid.NewGuid().ToString() };
                options.ChannelOptions = new MqttClientTcpOptions()
                {
                    Server = "emqx",
                    Port = 1883
                };

                options.CleanSession = true;
                options.KeepAlivePeriod = TimeSpan.FromSeconds(100.5);

                client = new MqttFactory().CreateMqttClient();

                client.UseApplicationMessageReceivedHandler(async (args) =>
                {

                    var sendingItem = new StringContent(JsonSerializer.Serialize(args.ApplicationMessage.ConvertPayloadToString()), Encoding.UTF8, "application/json");
                    string tmp = args.ApplicationMessage.ConvertPayloadToString();
                    int tmpBeggining = tmp.IndexOf("Sensor\":\"");
                    string sensor = tmp.Substring(tmpBeggining + 9, 17);
                    bool hightemp = false;
                    if (sensor.Equals("1c:bf:ce:15:ec:4d"))
                    {
                        hightemp = true;
                    }
                    sensor = sensor.Replace(':', '-');
                    if (args.ApplicationMessage.Topic.Equals("HighTemp"))
                    {
                        redis.AddItemToList(sensor,DateTime.Now.ToString()+ " HighTemp");
                        redis.AddItemToList("AllEvents", sensor + " " + DateTime.Now.ToString() +" " +  "HigherTemp");
                        await connection.InvokeAsync("SendMessage", "EventGroup", sensor + " " + DateTime.Now.ToString() + " " + "HigherTemp");

                        if (hightemp)
                            this.httpClient.PostAsync("http://hightempandhumiditysensorms:80/Control/AirCondition", sendingItem);
                        else
                            this.httpClient.PostAsync("http://stableconditionssensorms:80/Control/AirCondition", sendingItem);
                    }
                    if (args.ApplicationMessage.Topic.Equals("LowTemp"))
                    {
                        redis.AddItemToList(sensor, DateTime.Now.ToString() + " LowTemp");
                        redis.AddItemToList("AllEvents", sensor + " " + DateTime.Now.ToString() + " LowTemp");
                        await connection.InvokeAsync("SendMessage", "EventGroup", sensor + " " + DateTime.Now.ToString() + " " + "LowTemp");

                        if (hightemp)
                            this.httpClient.PostAsync("http://hightempandhumiditysensorms:80/Control/CentralHeating", sendingItem);
                        else
                            this.httpClient.PostAsync("http://stableconditionssensorms:80/Control/CentralHeating", sendingItem);
                    }
                    if (args.ApplicationMessage.Topic.Equals("HighHumidity"))
                    {
                        redis.AddItemToList(sensor, DateTime.Now.ToString() + " HighHumidity");
                        redis.AddItemToList("AllEvents", sensor + " " + DateTime.Now.ToString() + " HighHumidity");
                        await connection.InvokeAsync("SendMessage", "EventGroup", sensor + " " + DateTime.Now.ToString() + " " + "HighHumidity");


                        if (hightemp)
                            this.httpClient.PostAsync("http://hightempandhumiditysensorms:80/Control/Dehumidifier", sendingItem);
                        else
                            this.httpClient.PostAsync("http://stableconditionssensorms:80/Control/Dehumidifier", sendingItem);
                    }
                    if (args.ApplicationMessage.Topic.Equals("LowHumidity"))
                    {
                        redis.AddItemToList(sensor, DateTime.Now.ToString() + " LowHumidity");
                        redis.AddItemToList("AllEvents", sensor + " " + DateTime.Now.ToString() + " LowHumidity");
                        await connection.InvokeAsync("SendMessage", "EventGroup", sensor + " " + DateTime.Now.ToString() + " " + "LowHumidity");


                        if (hightemp)
                            this.httpClient.PostAsync("http://hightempandhumiditysensorms:80/Control/Humidifier", sendingItem);
                        else
                            this.httpClient.PostAsync("http://stableconditionssensorms:80/Control/Humidifier", sendingItem);
                    }
                    if (args.ApplicationMessage.Topic.Equals("Movement"))
                    {
                        redis.AddItemToList(sensor, DateTime.Now.ToString() + " Movement");
                        redis.AddItemToList("AllEvents", sensor + " " + DateTime.Now.ToString() + " Movement");
                        await connection.InvokeAsync("SendMessage", "EventGroup", sensor + " " + DateTime.Now.ToString() + " " + "Movement");


                        if (hightemp)
                            this.httpClient.PostAsync("http://hightempandhumiditysensorms:80/Control/Alarm", sendingItem);
                        else
                            this.httpClient.PostAsync("http://stableconditionssensorms:80/Control/Alarm", sendingItem);
                    }

                    Console.WriteLine("RECEIVED MESSAGE: " + Encoding.UTF8.GetString(args.ApplicationMessage.Payload) + "From topic: "+args.ApplicationMessage.Topic);

                });

                client.UseConnectedHandler(async (a) =>
                {
                    Console.WriteLine("### CONNECTED WITH SERVER ###");

                    // Subscribe to a topic
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("HighTemp").Build());
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("LowTemp").Build());
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("HighHumidity").Build());
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("LowHumidity").Build());
                    await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("Movement").Build());

                    Console.WriteLine("### SUBSCRIBED ###");
                });

            await client.ConnectAsync(options);
                Console.WriteLine("STARTED " + client.IsConnected);
        }
    }
}
