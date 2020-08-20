using Microsoft.AspNetCore.Mvc;

namespace WebApiVersionByUrl.Controllers.V3
{
    //[Route("api/v{version:apiVersion}/helloworld")]
    [ApiVersion("3.0")]
    [ApiVersion("3.1")]
    [ApiController]
    public class HelloWorldController : ControllerBase
    {
        [HttpPost]
        [Route("api/v3.0/helloworld/post")]
        [AdvertiseApiVersions]
        [MapToApiVersion("3.0")]
        public IActionResult Post(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 3.0.");

        [HttpPost]
        [Route("api/v3.1/helloworld/post")]
        [MapToApiVersion("3.1")]
        public IActionResult PostV3_1(ApiVersion apiVersion) =>
            Ok($"This is a post method, major version is {apiVersion.MajorVersion}," +
                $"minor Version is {apiVersion.MinorVersion},status is {apiVersion.Status}," +
                $"group version is {apiVersion.GroupVersion}.Hello from version 3.1.");
    }
}
