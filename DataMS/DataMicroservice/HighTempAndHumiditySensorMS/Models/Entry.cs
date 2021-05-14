using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HighTempAndHumiditySensorMS.Models
{
    public class Entry
    {
        private double _co;
        private double _humidity;
        private bool _light;
        private double _lpg;
        private bool _motion;
        private double _smoke;
        private double _temp;
        private double _mS;
        private string _sensor;

        public double Co { get => _co; set => _co = value; }
        public double Humidity { get => _humidity; set => _humidity = value; }
        public bool Light { get => _light; set => _light = value; }
        public double Lpg { get => _lpg; set => _lpg = value; }
        public bool Motion { get => _motion; set => _motion = value; }
        public double Smoke { get => _smoke; set => _smoke = value; }
        public double Temp { get => _temp; set => _temp = value; }
        public double MS { get => _mS; set => _mS = value; }
        public string Sensor { get => _sensor; set => _sensor = value; }

        public Entry()
        {
            
        }

        public Entry(double co, double humidity, bool light, double lpg, bool motion, double smoke, double temp, double mS, string device)
        {
            Co = co;
            Humidity = humidity;
            Light = light;
            Lpg = lpg;
            Motion = motion;
            Smoke = smoke;
            Temp = temp;
            MS = mS;
            Sensor = device;
        }
    }
}
