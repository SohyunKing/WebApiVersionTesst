using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace WebApiVersionByHeader.Controllers.V2_0
{
    [Route("api/helloworld")]
    //[ApiVersion("2.0")]
    [ApiController]
    public class WebApiVersionByQueryString : ControllerBase
    {
        [HttpPost]
        [Route("post")]
        //[MapToApiVersion("2.0")]
        [SwaggerParameter(name: "v", description: "api version", ParameterLocation = ParameterLocation.Query)]
        public IActionResult Post(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 2.0.");

        [HttpGet]
        [Route("get")]
        //[SwaggerParameter(name:"v",description:"api version", ParameterLocation = ParameterLocation.Query)]
        //[MapToApiVersion("2.0")]
        public IActionResult Get() => Ok("This is a get method, version 2.0.Hello from version 2.0.");
    }
}
