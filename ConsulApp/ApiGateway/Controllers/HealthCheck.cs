using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthCheck : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            var msg = Assembly.GetExecutingAssembly().GetName().Name + " is up & running!";
            return StatusCode(StatusCodes.Status200OK, msg);
        }
    }
}
