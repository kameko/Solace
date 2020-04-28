
namespace Caesura.Solace.Entities.Core.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    
    public class LogElementContext : DbContext, ISearchable<LogElement>
    {
        public string connection_string { get; private set; }
        public DbSet<LogElement> LogElements { get; set; }
        
        #nullable disable
        public LogElementContext(string path) : base()
        {
            connection_string = path;
        }
        
        public LogElementContext(DbContextOptions<LogElementContext> options) : base(options)
        {
            
        }
        #nullable restore
        
        public bool Search(string field, string term, int limit, out IEnumerable<LogElement> result)
        {
            var elms = new List<LogElement>(limit);
            if (field == "name")
            {
                var db_elms = LogElements.Where(x => x.Name == term);
                elms.AddRange(db_elms);
            }
            else if (field == "message")
            {
                var newterm = term.ToLower();
                var db_elms = LogElements.Where(x => 
                    x.Message.ToLower().Contains(newterm));
                elms.AddRange(db_elms);
            }
            else if (field == "before")
            {
                var dt_success = DateTime.TryParse(term, out var dt);
                if (dt_success)
                {
                    var db_elms = LogElements.Where(x => x.TimeStamp < dt).Take(limit);
                    elms.AddRange(db_elms);
                }
                else
                {
                    result = elms;
                    return false;
                }
            }
            else if (field == "after")
            {
                var dt_success = DateTime.TryParse(term, out var dt);
                if (dt_success)
                {
                    var db_elms = LogElements.Where(x => x.TimeStamp > dt).Take(limit);
                    elms.AddRange(db_elms);
                }
                else
                {
                    result = elms;
                    return false;
                }
            }
            else if (field == "exception-name")
            {
                var db_elms = LogElements.Where(x => x.Exception.Name == term);
                elms.AddRange(db_elms);
            }
            else if (field == "exception-message")
            {
                var newterm = term.ToLower();
                var db_elms = LogElements.Where(x => 
                    x.Exception.Message.ToLower().Contains(newterm));
                elms.AddRange(db_elms);
            }
            else
            {
                result = elms;
                return false;
            }
            
            result = elms;
            return true;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(connection_string);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
