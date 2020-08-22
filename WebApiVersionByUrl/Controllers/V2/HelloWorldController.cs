using Microsoft.AspNetCore.Mvc;

namespace WebApiVersionByUrl.Controllers.V2
{
    [Route("api/v2.0/helloworld")]
    [ApiVersion("2.0")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        [Route("get")]
        public IActionResult Get() => Ok("This is a get method, version 1.0.Hello from version 1.0.");

        [HttpPost]
        [Route("post")]
        public IActionResult Post(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 2.0.");
    }
}
