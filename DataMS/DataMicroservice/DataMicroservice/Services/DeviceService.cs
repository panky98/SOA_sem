using ServiceStack.Redis;
using StableConditionsSensorMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataMicroservice.Services
{
    public class DeviceService
    {
        readonly RedisClient redis = new RedisClient("redis-api",6379);
        public IList<Entry> GetEntries(string sensorStandardMac)
        {
            long numberOfInputs = long.Parse(redis.GetValueFromHash("devices", sensorStandardMac));
            IList<Entry> entries = new List<Entry>();
            for (long i = 1; i <= numberOfInputs; i++)
            {
                double co;
                double humidity;
                bool light;
                double lpg;
                bool motion;
                double smoke;
                double temp;
                double ms;
                string sensor;
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Co"), out co);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Humidity"), out humidity);
                Boolean.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Light"), out light);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Lpg"), out lpg);
                Boolean.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Motion"), out motion);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Smoke"), out smoke);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Temp"), out temp);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "MS"), out ms);
                sensor = redis.GetValueFromHash(sensorStandardMac + ":" + i, "Sensor");
                Entry entry = new Entry(co, humidity, light, lpg, motion, smoke, temp, ms, sensor);
                entries.Add(entry);
            }
            return entries;
        }
        public IList<Entry> GetRangeEntries(string sensorStandardMac,string attributeName,double lowerBound,double upperBound)
        {
            long numberOfInputs = long.Parse(redis.GetValueFromHash("devices", sensorStandardMac));
            IList<Entry> entries = new List<Entry>();
            for (long i = 1; i <= numberOfInputs; i++)
            {
                double co;
                double humidity;
                bool light;
                double lpg;
                bool motion;
                double smoke;
                double temp;
                double ms;
                string sensor;
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Co"), out co);
                if (attributeName.Equals("Co") && !(co > lowerBound && co < upperBound))
                    continue;
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Humidity"), out humidity);
                if (attributeName.Equals("Humidity") && !(humidity > lowerBound && humidity < upperBound))
                    continue;
                Boolean.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Light"), out light);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Lpg"), out lpg);
                if (attributeName.Equals("Lpg") && !(lpg > lowerBound && lpg < upperBound))
                    continue;
                Boolean.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Motion"), out motion);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Smoke"), out smoke);
                if (attributeName.Equals("Smoke") && !(smoke > lowerBound && smoke < upperBound))
                    continue;
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Temp"), out temp);
                if (attributeName.Equals("Temp") && !(temp > lowerBound && temp < upperBound))
                    continue;
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "MS"), out ms);
                if (attributeName.Equals("MS") && !(ms > lowerBound && ms < upperBound))
                    continue;
                sensor = redis.GetValueFromHash(sensorStandardMac + ":" + i, "Sensor");
                Entry entry = new Entry(co, humidity, light, lpg, motion, smoke, temp, ms, sensor);
                entries.Add(entry);
            }
            return entries;
        }
        public void addRow(Entry deviceData)
        {
            string standardMac = deviceData.Sensor.Replace(':', '-');
            redis.IncrementValueInHash("devices", standardMac, 1);
            long numberOfDevices = redis.GetHashCount("devices");
            string deviceInputNumber = redis.GetValueFromHash("devices", standardMac);
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Co", deviceData.Co.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Humidity", deviceData.Humidity.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Light", deviceData.Light.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Lpg", deviceData.Lpg.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Motion", deviceData.Motion.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "MS", deviceData.MS.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Sensor", deviceData.Sensor.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Smoke", deviceData.Smoke.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Temp", deviceData.Temp.ToString());
        }
    }
}
