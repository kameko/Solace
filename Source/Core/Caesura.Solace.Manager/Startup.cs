
namespace Caesura.Solace.Manager
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
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
            var path   = Configuration[$"Manager:DatabasePath"];
            var constr = Configuration[$"Manager:ConnectionString"].Replace("{DatabasePath}", path);
            
            services.AddDbContext<LogElementContext>(opt =>
            {
                opt.UseSqlite(constr);
            });
            
            services.AddScoped<ILogService, LogService>();
            
            services.AddControllers();
            // TODO: services.AddOData();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
