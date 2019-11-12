using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using DDDEfCore.ProductCatalog.Services.Commands;
using DDDEfCore.ProductCatalog.Services.Queries;
using DDDEfCore.ProductCatalog.WebApi.Infrastructures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DDDEfCore.ProductCatalog.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState.Values
                                .SelectMany(x => x.Errors.Select(p => p.ErrorMessage))
                                .ToList();
                        var result = new
                        {
                            Code = HttpStatusCode.BadRequest,
                            Message = "Validation errors",
                            Errors = errors
                        };
                        return new BadRequestObjectResult(result);
                    };
                });
            services.AddSingleton<IConfiguration>(sp => this.Configuration);
            services.AddApplicationCommands();
            services.AddApplicationQueries();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo
                {
                    Title = "ProductCatalog API",
                    Version = "V1",
                    Description = "A simple example ASP.NET Core Web API",
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/V1/swagger.json", "ProductCatalog API");
                c.RoutePrefix = string.Empty;
            });
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}


/*
 * Resources:
 *  1. https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.0&tabs=visual-studio
 *  2. Notes:
 *      c.SwaggerDoc("V1", new OpenApiInfo
 *      => c.SwaggerEndpoint("/swagger/V1/swagger.json", "ProductCatalog API");
 *      => V1 must be matched
 *  3. GlobalExceptionMiddleware
 *      https://code-maze.com/global-error-handling-aspnetcore/
 */
