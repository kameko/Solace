
namespace Solace.Core.Services.Database
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IDatabaseService : IService
    {
        Task Save<T>(T item) where T : class;
        Task SaveMany<T>(IEnumerable<T> items) where T : class;
        Task<IEnumerable<T>> Query<T>(Predicate<T> predicate) where T : class;
        Task<IEnumerable<T>> Query<T>(Func<IEnumerable<T>, IEnumerable<T>> db) where T : class;
    }
}
