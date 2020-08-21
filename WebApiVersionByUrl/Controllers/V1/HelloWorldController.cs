using Microsoft.AspNetCore.Mvc;

namespace WebApiVersionByUrl.Controllers.V1
{
    //[ApiVersion("1.0", Deprecated = true)]
    //[ApiVersion("1.1")]
    [Route("api/v{version:apiVersion}/helloworld")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        [Route("get")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        public IActionResult Get() => Ok("This is a get method, version 1.0.Hello from version 1.0.");

        [HttpPost]
        [Route("post")]
        //[MapToApiVersion("1.0")]
        public IActionResult Post(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 1.0.");

        [HttpPost]
        [Route("~/api/v1.1/helloworld/post")]
        //[MapToApiVersion("1.1")]
        public IActionResult PostV1_1(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 1.1.");
    }
}
