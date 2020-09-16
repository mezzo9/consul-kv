using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OrderService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly WhereAmI _where;
        private readonly string  _appPort;
        public OrderController(IConfiguration configuration)
        {
            _where = configuration.GetSection("WhereAmI").Get<WhereAmI>();
            _appPort = configuration.GetValue<string>("WebServicePort");
        }
        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[]
            {
                System.Environment.MachineName, 
                _where.CurrentEnvironment ?? string.Empty,
                _where.DataCenter ?? string.Empty,
                _appPort
            };
        }

        // GET api/<OrderController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

 
    }

    public class WhereAmI
    {
        public string CurrentEnvironment { get; set; }
        public string DataCenter { get; set; }

    }
}
