
namespace Solace.Core.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    
    public abstract class DatabaseService : BaseService, IDatabaseService
    {
        public virtual Task Save<T>(T item) where T : class
        {
            return Task.CompletedTask;
        }
        
        public virtual Task SaveMany<T>(IEnumerable<T> items) where T : class
        {
            return Task.CompletedTask;
        }
        
        public virtual Task<IEnumerable<T>> Query<T>(Predicate<T> predicate) where T : class
        {
            return (Task.FromResult(new List<T>()) as Task<IEnumerable<T>>)!;
        }
        
        public virtual Task<IEnumerable<T>> Query<T>(DbSet<T> db) where T : class
        {
            return (Task.FromResult(new List<T>()) as Task<IEnumerable<T>>)!;
        }
    }
}
