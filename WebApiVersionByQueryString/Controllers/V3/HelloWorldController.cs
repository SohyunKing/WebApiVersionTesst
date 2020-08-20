using Microsoft.AspNetCore.Mvc;

namespace WebApiVersionTest.Controllers.V3
{
    [Route("api/helloworld")]
    //[ApiVersion("3.0")]
    //[ApiVersion("3.1")]
    [ApiController]
    public class WebApiVersionByQueryString : ControllerBase
    {
        [HttpPost]
        [Route("post")]
        [MapToApiVersion("3.0")]
        //[SwaggerParameter(name: "v", description: "api version", ParameterType = "string")]
        public IActionResult Post(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 3.0.");

        [HttpGet]
        [Route("get")]
        public IActionResult Get(ApiVersion apiVersion) => Ok($"This is a get method, " +
            $"version {apiVersion.MajorVersion}.{apiVersion.MinorVersion}" +
            "Hello from version 1.0.");

        [HttpPost]
        [Route("post")]
        [MapToApiVersion("3.1")]
        //[SwaggerParameter(name: "v", description: "api version", ParameterType = "string")]
        public IActionResult PostV3_1(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 3.1.");
    }
}
