
namespace Caesura.Solace.Entities.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    
    public class LogElement
    {
        public long Id { get; set; }
        
        public DateTime TimeStamp { get; set; }
        public LogLevel Level { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
    }
    
    #nullable disable
    public class LogElementContext : DbContext
    {
        public LogElementContext(DbContextOptions<LogElementContext> options)
            : base(options)
        {
            
        }

        public DbSet<LogElement> LogItems { get; set; }
    }
    #nullable restore
}
