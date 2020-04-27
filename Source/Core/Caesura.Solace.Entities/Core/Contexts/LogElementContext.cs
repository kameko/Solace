
namespace Caesura.Solace.Entities.Core.Contexts
{
    using System;
    using Microsoft.EntityFrameworkCore;
    
    #nullable disable
    public class LogElementContext : DbContext
    {
        public DbSet<LogElement> LogItems { get; set; }
        
        public LogElementContext() : base()
        {
            
        }
        
        public LogElementContext(DbContextOptions<LogElementContext> options) : base(options)
        {
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=./Data/Log/log.db; Version=3; Journal Mode=Persist");
        }
    }
    #nullable restore
}
