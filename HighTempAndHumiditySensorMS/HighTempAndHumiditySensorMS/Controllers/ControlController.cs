using HighTempAndHumiditySensorMS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HighTempAndHumiditySensorMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ControlController : Controller
    {
        private readonly SensorService _sensor;
        public ControlController(SensorService _sensor)
        {
            this._sensor = _sensor;
        }
    }
}
