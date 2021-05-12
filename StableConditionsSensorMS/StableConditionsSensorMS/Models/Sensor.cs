using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StableConditionsSensorMS.Models
{
    public class Sensor
    {
        private string _device;
        private string setup;

        public string Setup { get => setup; set => setup = value; }
        public string Device { get => _device; set => _device = value; }

        public Sensor()
        {

        }

        public Sensor(string device,string setup)
        {
            Setup = setup;
            Device = device;
        }
    }
}
