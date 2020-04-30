
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
    using Controllers.Interfaces;
    using Controllers.Services;
    
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var path   = new FileInfo(Configuration[$"Data:DatabasePath"]);
            var constr = Configuration[$"Data:ConnectionString"].Replace("{DatabasePath}", path.FullName);
            
            services.AddDbContext<LogElementContext>(opt =>
            {
                opt.UseSqlite(constr);
            });
            
            services.AddScoped<ILogService, LogService>();
            
            services.AddMvc(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            });
            
            services.AddControllers()
                // TODO: remove this when possible in favor of System.Text.Json. Maybe in .NET 5
                .AddNewtonsoftJson(); // what GODDAMN IDIOT working on OData is responsible for this?
            services.AddOData();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!int.TryParse(Configuration[$"Data:GetLimit"], out var get_limit))
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
                    .Filter()
                    .MaxTop(get_limit);
            });
        }
    }
}
