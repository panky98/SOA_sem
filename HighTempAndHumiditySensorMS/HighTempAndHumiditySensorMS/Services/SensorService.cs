using HighTempAndHumiditySensorMS.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public SensorService()
        {
            threshold = 0.54f;//10% initial threshold
            lightThreshold = true;
            motionThreshold = true;
            csvFile = new StreamReader("..\\iot_telemetry_data.csv");
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
                MS = Convert.ToDouble(entryList[0])
            };

            lastValue = new Entry(lastValueReferent.Co, lastValueReferent.Humidity, lastValueReferent.Light, lastValueReferent.Lpg, 
                                    lastValueReferent.Motion, lastValueReferent.Smoke, lastValueReferent.Temp, lastValueReferent.MS);

        }

        public Task StartAsync(CancellationToken cancellationToken)
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
                    MS = Convert.ToDouble(entryList[0])
                };

                double diff = newValue.MS - lastValue.MS;
                Thread.Sleep(Convert.ToInt32(diff));
                
                lastValue = newValue;
                if(ChangedForThresHold(newValue,entryList))
                {
                    lastValueReferent = newValue;
                    //pozvati DataMS kao REST CLIENT
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
            if (lightThreshold && val.Light != lastValueReferent.Light)
                return true;
            if (motionThreshold && val.Motion != lastValueReferent.Motion)
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
                if (percentage > threshold)
                    return true;
            }

            return false;
        }
    }
}
