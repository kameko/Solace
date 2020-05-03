
namespace Caesura.Solace.Manager
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNet.OData.Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Entities.Core.Manager.Contexts;
    using Foundation;
    using Foundation.ApiBoundaries.HttpClients.Core.Database;
    using Controllers.Interfaces;
    using Controllers.Services;
    using ServiceManagement;
    
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: verify these config items are valid.
            var path   = new FileInfo(Configuration[$"Storage:Log:Path"]);
            var constr = Configuration[$"Storage:Log:ConnectionString"].Replace("{Path}", path.FullName);
            
            services.AddDbContext<LogElementContext>(opt =>
            {
                opt.UseSqlite(constr);
            });
            
            services.AddScoped<ILogService, LogService>();
            
            services.AddSingleton<ISolaceServiceCollection, SolaceServiceCollection>();
            
            services.AddHttpClient<DatabaseClient>();
            
            services.AddHostedService<LifetimeEventsHostedService>();
            services.AddHostedService<ServiceManagerHostedService>();
            
            services.AddMvc(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            });
            
            services.AddHttpClient();
            
            services.AddControllers()
                // TODO: remove this when possible in favor of System.Text.Json. Maybe in .NET 5
                .AddNewtonsoftJson(); // what GODDAMN IDIOT working on OData is responsible for this?
            services.AddOData();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!int.TryParse(Configuration[$"Networking:GetLimit"], out var get_limit))
            {
                get_limit = 100;
            }
            
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
