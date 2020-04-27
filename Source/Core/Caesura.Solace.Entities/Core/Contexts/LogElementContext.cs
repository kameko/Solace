
namespace Caesura.Solace.Entities.Core.Contexts
{
    using System;
    using Microsoft.EntityFrameworkCore;
    
    #nullable disable
    public class LogElementContext : DbContext
    {
        public DbSet<LogElement> LogItems { get; set; }
        
        public LogElementContext(DbContextOptions<LogElementContext> options) : base(options)
        {
            
        }
    }
    #nullable restore
}
