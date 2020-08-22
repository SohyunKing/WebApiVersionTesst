using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using WebApiVersionByUrl.Controllers.V1;

namespace WebApiVersionByUrl
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMvc(c =>
            c.Conventions.Add(
            new ApiExplorerGroupPerVersionConvention()) // decorate Controllers to distinguish SwaggerDoc (v1, v2, etc.)
            );

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;

                options.Conventions.Add(new VersionByNamespaceConvention());

                //options.Conventions.Controller<HelloWorldController>().
                //    HasApiVersion(1, 0).
                //    Action<HelloWorldController>(c => c.Get()).
                //    MapToApiVersion(1, 1);
                ////header
                //options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                ////query string
                //options.ApiVersionReader = new QueryStringApiVersionReader("v");
                //route
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                //combine
                //options.ApiVersionReader = ApiVersionReader.Combine(
                //    new QueryStringApiVersionReader("v"),
                //    new UrlSegmentApiVersionReader(),
                //    new HeaderApiVersionReader("api-version"));
            });
            //services.Remove(services.Single(s => s.ImplementationType == typeof(ApiVersionMatcherPolicy)));
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, FallbackApiVersionMatcherPolicy>());
            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Swagger Document",
                    Description = "A Web API",
                    TermsOfService = new Uri("https://example.com/terms"),

                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "Swagger Document",
                    Description = "A Web API",
                    TermsOfService = new Uri("https://example.com/terms"),

                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                c.SwaggerDoc("v3", new OpenApiInfo
                {
                    Version = "v3",
                    Title = "Swagger Document",
                    Description = "A Web API",
                    TermsOfService = new Uri("https://example.com/terms"),

                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license"),
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger V1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Swagger V2");
                c.SwaggerEndpoint("/swagger/v3/swagger.json", "Swagger V3");
                //c.RoutePrefix = string.Empty;
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
        {
            public void Apply(ControllerModel controller)
            {
                var controllerNamespace = controller.ControllerType.Namespace; // e.g. "Controllers.v1"
                var apiVersion = controllerNamespace?.Split('.').Last().ToLower();
                controller.ApiExplorer.GroupName = apiVersion;
            }
        }
    }
}
