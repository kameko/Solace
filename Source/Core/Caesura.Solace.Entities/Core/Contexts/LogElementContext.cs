
namespace Caesura.Solace.Entities.Core.Contexts
{
    using System;
    using Microsoft.EntityFrameworkCore;
    
    #nullable disable
    public class LogElementContext : DbContext
    {
        public string connection_string { get; private set; }
        public DbSet<LogElement> LogElements { get; set; }
        
        public LogElementContext(string path) : base()
        {
            connection_string = path;
        }
        
        public LogElementContext(DbContextOptions<LogElementContext> options) : base(options)
        {
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(connection_string);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
    #nullable restore
}
