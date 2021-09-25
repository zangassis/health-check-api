using HealthCheck.Custom;
using HealthCheck.Helpers;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace HealthCheck
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
            //Here is HealthCheck and the connection to Redis
            services.AddHealthChecks()
                .AddRedis(redisConnectionString: UtilsHelpers.GetConnectionString(), name: "Redis instance")
                .AddCheck<CustomHealthChecks>("Custom Health Checks"); //Here is the custom class dependency injection

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthCheck", Version = "v1" });
            });

            // Here is the GUI setup and history storage
            services.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(5); //Sets the time interval in which HealthCheck will be triggered
                options.MaximumHistoryEntriesPerEndpoint(10); //Sets the maximum number of records displayed in history
                options.AddHealthCheckEndpoint("Health Checks API", "/health"); //Sets the Health Check endpoint
            }).AddInMemoryStorage(); //Here is the memory bank configuration
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthCheck v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                //Sets the health endpoint
                endpoints.MapHealthChecks("/health");
            });

            //Sets Health Check dashboard options
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = p => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            //Sets the Health Check dashboard configuration
            app.UseHealthChecksUI(options => { options.UIPath = "/dashboard"; });
        }
    }
}
