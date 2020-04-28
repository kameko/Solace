
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System.Threading.Tasks;
    
    public interface IControllerService
    {
        
    }
    
    public interface IControllerService<TKey, T, TSource> : IControllerService
    {
        Task<ControllerResult.GetAll<T>> GetAll();
        Task<ControllerResult.GetById<T>> GetById(TKey id);
        Task<ControllerResult.Put> Put(TKey id, T value);
        Task<ControllerResult.Post<T>> Post(T value);
        Task<ControllerResult.Delete> Delete(TKey id);
    }
    
    public interface IControllerSearchableService<T, TTerm, TSource> : IControllerService
    {
        Task<ControllerResult.GetBySearch<T>> GetBySearch(string field, TTerm term);
    }
}
