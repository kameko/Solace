
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System.Threading.Tasks;
    
    public interface IControllerService
    {
        
    }
    
    public interface IControllerService<TKey, T, TSource> : IControllerService
    {
        // TODO: Consider a GetOne() method that returns just T and
        // not IEnumerable<T> like GetAll() does.
        Task<ControllerResult.GetAll<T>> GetAll();
        Task<ControllerResult.GetById<T>> GetById(TKey id);
        Task<ControllerResult.Put> Put(TKey id, T value);
        Task<ControllerResult.Post<T>> Post(T value);
        Task<ControllerResult.DeleteAll> DeleteAll();
        Task<ControllerResult.DeleteById> DeleteById(TKey id);
    }
    
    public interface IControllerSearchableService<TKey, T, TTerm, TSource> : IControllerService<TKey, T, TSource> 
    {
        Task<ControllerResult.GetBySearch<T>> GetBySearch(string field, TTerm term);
    }
}
