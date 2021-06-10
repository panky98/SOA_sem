using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommandController : ControllerBase
    {
        readonly RedisClient redis = new RedisClient("redis-api-events", 6379);

        [HttpGet]
        [Route("getEventsFromSensor/{sensorStandardMac}")]
        public async Task<IActionResult> getAllSensorData([FromRoute(Name = "sensorStandardMac")] string sensorStandardMac)
        {
            if(redis.Exists(sensorStandardMac)==1)
            {
                IList<string> returnList = redis.GetAllItemsFromList(sensorStandardMac);
                return new JsonResult(returnList);
            }
            return new EmptyResult();
        }

        [HttpGet]
        [Route("getAllGeneratedEvents")]
        public async Task<IActionResult> getAllGeneratedEvents()
        {
            if (redis.Exists("AllEvents") == 1)
            {
                IList<string> returnList = redis.GetAllItemsFromList("AllEvents");
                return new JsonResult(returnList);
            }
            return new EmptyResult();
        }

    }
}
