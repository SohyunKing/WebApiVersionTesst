using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiVersionByUrl.Controllers
{
    [ApiController]
    [ApiVersionNeutral]
    [Route("api/v{version:apiVersion}/helloworld")]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        [Route("get")]
        //[MapToApiVersion("1.0")]
        public IActionResult Get() => Ok("This is a get method, version neutral.");
    }
}
