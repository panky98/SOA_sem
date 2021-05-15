using HighTempAndHumiditySensorMS.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HighTempAndHumiditySensorMS.Services
{
    public class SensorService : IHostedService
    {
        private StreamReader csvFile;
        private Entry lastValueReferent;
        private Entry lastValue;
        private float threshold;
        private bool lightThreshold;
        private bool motionThreshold;
        private readonly IHttpClientFactory _httpFactory;
        private HttpClient client;


        public bool MotionThreshold { get => motionThreshold; set => motionThreshold = value; }
        public bool LightThreshold { get => lightThreshold; set => lightThreshold = value; }
        public float Threshold { get => threshold; set => threshold = value; }

        public SensorService(IHttpClientFactory _httpFactory)
        {
            this._httpFactory = _httpFactory;
            this.client = _httpFactory.CreateClient();
            Threshold = 0.54f;//10% initial threshold
            LightThreshold = true;
            MotionThreshold = true;
            csvFile = new StreamReader("iot_telemetry_data.csv");
            csvFile.ReadLine();
            string line;
            IList<string> entryList = null;
            while ((line=csvFile.ReadLine())!=null)
            {
                line= line.Replace("\"", "");
                entryList = line.Split(",");
                if(entryList[1].Equals("1c:bf:ce:15:ec:4d"))
                {
                    break;
                }
            }

            lastValueReferent = new Entry()
            {
                Co = Convert.ToDouble(entryList[2]),
                Humidity = Convert.ToDouble(entryList[3]),
                Light = Convert.ToBoolean(entryList[4]),
                Lpg = Convert.ToDouble(entryList[5]),
                Motion = Convert.ToBoolean(entryList[6]),
                Smoke = Convert.ToDouble(entryList[7]),
                Temp = Convert.ToDouble(entryList[8]),
                MS = Convert.ToDouble(entryList[0]),
                Sensor = "1c:bf:ce:15:ec:4d"
            };

            lastValue = new Entry(lastValueReferent.Co, lastValueReferent.Humidity, lastValueReferent.Light, lastValueReferent.Lpg, 
                                    lastValueReferent.Motion, lastValueReferent.Smoke, lastValueReferent.Temp, lastValueReferent.MS,lastValueReferent.Sensor);

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(Routine, cancellationToken);
            return Task.CompletedTask;
        }


        public Task Routine()
        {
            string line;
            IList<string> entryList = null;
            while ((line = csvFile.ReadLine()) != null)
            {
                line = line.Replace("\"", "");
                entryList = line.Split(",");
                if (!entryList[1].Equals("1c:bf:ce:15:ec:4d"))
                {
                    continue;
                }

                Entry newValue = new Entry()
                {
                    Co = Convert.ToDouble(entryList[2]),
                    Humidity = Convert.ToDouble(entryList[3]),
                    Light = Convert.ToBoolean(entryList[4]),
                    Lpg = Convert.ToDouble(entryList[5]),
                    Motion = Convert.ToBoolean(entryList[6]),
                    Smoke = Convert.ToDouble(entryList[7]),
                    Temp = Convert.ToDouble(entryList[8]),
                    MS = Convert.ToDouble(entryList[0]),
                    Sensor = "1c:bf:ce:15:ec:4d"
                };

                double diff = newValue.MS - lastValue.MS;
                Task.Delay(Convert.ToInt32(diff * 1000)).Wait();

                lastValue = newValue;
                if (ChangedForThresHold(newValue, entryList))
                {
                    lastValueReferent = newValue;
                    var sendingItem = new StringContent(JsonSerializer.Serialize(newValue), Encoding.UTF8, "application/json");
                    this.client.PostAsync("http://datamicroservice:80/DataMicroservice/addRow", sendingItem);
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            csvFile.Close();
            return Task.CompletedTask;
        }
        public bool ChangedForThresHold(Entry val,IList<string> entries)
        {
            //boolean podaci da li su se promenili
            //i da li je ukljucen monitoring njihov
            if (LightThreshold && val.Light != lastValueReferent.Light)
                return true;
            if (MotionThreshold && val.Motion != lastValueReferent.Motion)
                return true;

            return ChangedForThresHoldDoubles(entries);
        }

        public bool ChangedForThresHoldDoubles(IList<string> entries)
        {
            IDictionary<int, double> refValues = new Dictionary<int, double>();
            refValues.Add(2, lastValueReferent.Co);
            refValues.Add(3, lastValueReferent.Humidity);
            refValues.Add(5, lastValueReferent.Lpg);
            refValues.Add(7, lastValueReferent.Smoke);
            refValues.Add(8, lastValueReferent.Temp);



            for (int i=2;i<entries.Count;i++)
            {
                if (i == 4 || i == 6)
                    continue;

                double value = Convert.ToDouble(entries[i]);
                double percentage = 100 * value / refValues[i];
                percentage = Math.Abs(percentage - 100);
                if (percentage > Threshold)
                    return true;
            }

            return false;
        }
    }
}
