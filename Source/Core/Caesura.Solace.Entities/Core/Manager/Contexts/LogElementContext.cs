
namespace Caesura.Solace.Entities.Core.Manager.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    
    public class LogElementContext : DbContext, ISearchable<LogElement>
    {
        public DbSet<LogElement> LogElements { get; set; }
        
        #nullable disable
        public LogElementContext() : base()
        {
            
        }
        
        public LogElementContext(DbContextOptions<LogElementContext> options) : base(options)
        {
            
        }
        #nullable restore
        
        public bool Search(string field, string term, int limit, out IEnumerable<LogElement> result)
        {
            var elms = new List<LogElement>(limit);
            if (field == "sender-service" || field == "sender" || field == "name")
            {
                var db_elms = LogElements.Where(x => x.SenderService == term);
                elms.AddRange(db_elms);
            }
            else if (field == "receiver-service" || field == "receiver")
            {
                var db_elms = LogElements.Where(x => x.ReceiverService == term);
                elms.AddRange(db_elms);
            }
            else if (field == "message")
            {
                var newterm = term.ToLower();
                var db_elms = LogElements.Where(x => 
                    x.Message.ToLower().Contains(newterm));
                elms.AddRange(db_elms);
            }
            else if (field == "element-exact" || field == "element")
            {
                // TODO:
                throw new NotImplementedException();
            }
            else if (field == "element-contains")
            {
                // TODO:
                throw new NotImplementedException();
            }
            else if (field == "range")
            {
                // TODO: parse {ulong1}-{ulong2} and get all messages with ID
                // greater than ulong1 but less than ulong2
                throw new NotImplementedException();
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
            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
