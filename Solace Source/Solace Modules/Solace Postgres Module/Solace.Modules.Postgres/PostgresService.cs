
namespace Solace.Modules.Postgres
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Core.Services.Database;
    
    // TODO:
    // https://www.connectionstrings.com/npgsql/ 
    // 
    
    public class PostgresService : SqlDatabaseService
    {
        public override Task Connect(string connection_string)
        {
            ConnectionString = connection_string;
            return Task.CompletedTask;
        }
        
        public override Task Save<T>(T item) where T : class
        {
            return Task.CompletedTask;
        }
        
        public override Task SaveMany<T>(IEnumerable<T> items) where T : class
        {
            return Task.CompletedTask;
        }
        
        public override Task<IEnumerable<T>> Query<T>(Predicate<T> predicate) where T : class
        {
            return (Task.FromResult(new List<T>()) as Task<IEnumerable<T>>)!;
        }
        
        public override Task<IEnumerable<T>> Query<T>(Func<IEnumerable<T>, IEnumerable<T>> db) where T : class
        {
            // NOTICE: the argument passed to the db delegate is meant to be a DbSet, which inherits IEnumerable
            return (Task.FromResult(new List<T>()) as Task<IEnumerable<T>>)!;
        }
    }
}
