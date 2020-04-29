
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System.Threading.Tasks;
    using Entities.Core.Contexts;
    
    public interface IControllerService<TKey, T, TSource> where T : IHasId<TKey>
    {
        Task<ControllerResult.GetOne<T>> GetOne();
        Task<ControllerResult.GetAll<T>> GetAll();
        Task<ControllerResult.GetById<T>> GetById(TKey id);
        Task<ControllerResult.Put> Put(TKey id, T value);
        Task<ControllerResult.Post<T>> Post(T value);
        Task<ControllerResult.DeleteAll> DeleteAll();
        Task<ControllerResult.DeleteById> DeleteById(TKey id);
    }
    
    public interface IControllerSearchableService<TKey, T, TTerm, TSource>
        : IControllerService<TKey, T, TSource>
        where T : IHasId<TKey>
    {
        Task<ControllerResult.GetBySearch<T>> GetBySearch(string field, TTerm term);
    }
}