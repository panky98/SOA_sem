using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StableConditionsSensorMS.Models;
using StableConditionsSensorMS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StableConditionsSensorMS.Controllers
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("GetThresHold")]
        public IActionResult GetThresHold()
        {
            return new OkObjectResult(this._sensor.Threshold);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("GetLightMonitoring")]
        public IActionResult GetLightMonitoring()
        {
            return new OkObjectResult(this._sensor.LightThreshold);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("GetMotionMonitoring")]
        public IActionResult GetMotionMonitoring()
        {
            return new OkObjectResult(this._sensor.MotionThreshold);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("SwitchMotionMonitoring")]

        public IActionResult SwitchMotionMonitoring()
        {
            try
            {
                this._sensor.MotionThreshold = !this._sensor.MotionThreshold;
                return Ok();
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToString());
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("SwitchLightMonitoring")]

        public IActionResult SwitchLightMonitoring()
        {
            try
            {
                this._sensor.LightThreshold = !this._sensor.LightThreshold;
                return Ok();
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToString());
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("ChangeThreshold")]

        public IActionResult ChangeThreshold([FromBody] float newThreshold)
        {
            try
            {
                this._sensor.Threshold = newThreshold;
                return Ok();
            }
            catch (Exception exc)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exc.ToString());
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("GetSensors")]
        public IActionResult GetSensors()
        {
            return new OkObjectResult(this._sensor.Sensors);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("GetCurrentSensor")]
        public IActionResult GetCurrentSensor()
        {
            return new OkObjectResult(this._sensor.Sensors[this._sensor.CurrSensorId]);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("ChangeSensor")]
        public IActionResult ChangeSensor()
        {
            this._sensor.CurrSensorId = (this._sensor.CurrSensorId + 1) % 2;
            this._sensor.CurrentSensor = this._sensor.Sensors[this._sensor.CurrSensorId].Device;
            return Ok();
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("AirCondition")]
        public IActionResult AirCondition()
        {
            Console.WriteLine("AirCondition");
            return Ok();
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("Humidifier")]
        public IActionResult Humidifier()
        {
            Console.WriteLine("Humidifier activated");
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("Dehumidifier")]
        public IActionResult Dehumidifier()
        {
            Console.WriteLine("Dehumidifier activated");
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("Alarm")]
        public IActionResult Alarm()
        {
            Console.WriteLine("Alarm activated");
            return Ok();
        }
    }
}
