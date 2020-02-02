
namespace Solace.Core.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    
    public abstract class DatabaseElement : DbContext
    {
        public Action<DbContextOptionsBuilder>? Builder { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Builder?.Invoke(optionsBuilder);
        }
    }
    
    public class DatabaseElement<T> : DatabaseElement where T : class
    {
        public DbSet<T>? DbItem { get; set; }
    }
}
