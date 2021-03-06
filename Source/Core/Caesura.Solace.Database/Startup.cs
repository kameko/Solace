
namespace Caesura.Solace.Database
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Foundation;
    using Foundation.ConfigurationModels;
    using Foundation.ApiBoundaries.HttpClients.Core.Manager;
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: add db context for entities
            
            services.AddHttpClient<ManagerClient>();
            
            services.AddHostedService<LifetimeEventsHostedService>();
            
            services.AddMvc(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            });
            
            services.AddHttpClient();
            
            services.AddControllers()
                // TODO: remove this when possible in favor of System.Text.Json. Maybe in .NET 5
                .AddNewtonsoftJson();
            services.AddOData();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var networking_model = Configuration.GetSection(ConfigurationConstants.Networking).Get<NetworkingModel>();
            var get_limit = networking_model.GetLimit;
            
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                routeBuilder // OData functionality list
                    .Expand()
                    .Select()
                    .Count()
                    .OrderBy()
                    .Filter();
                
                if (get_limit > 0)
                {
                    routeBuilder.MaxTop(get_limit);
                }
            });
        }
    }
}
