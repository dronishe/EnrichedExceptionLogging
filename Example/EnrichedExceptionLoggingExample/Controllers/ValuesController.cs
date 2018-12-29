using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLoggingExample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        public ILogger<ValuesController> Logger { get; }

        public ValuesController(ILogger<ValuesController> logger)
        {
            Logger = logger;
        }
       
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Logger.LogTrace("Information from get method!");
            throw new Exception("Smthg wrong!");
            return new string[] { "value1", "value2" };
        }
    }
}
