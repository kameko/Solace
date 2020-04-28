
namespace Caesura.Solace.Manager.Controllers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities.Core;
    
    // TODO: get rid of the specific Result classes, make generic
    // ones with a generic IService<K, T> and IService<T> where
    // the K argument defaults to ulong. Give them all the same
    // enum, like Put.ResultKind
    
    public interface ILogService
    {
        Task<LogServiceResult.GetAll> Get();
        Task<LogServiceResult.GetById> Get(ulong id);
        Task<LogServiceResult.GetBySearch> Get(string field, string term);
        Task<LogServiceResult.Put> Put(ulong id, LogElement element);
        Task<LogServiceResult.Post> Post(LogElement element);
        Task<LogServiceResult.Delete> Delete(ulong id);
    }
    
    public static class LogServiceResult
    {
        public sealed class GetAll
        {
            private IEnumerable<LogElement>? _val;
            
            public bool Success { get; private set; }
            public IEnumerable<LogElement> Value => Success && !(_val is null) ? _val : throw new InvalidOperationException();
            
            private GetAll(IEnumerable<LogElement> elms)
            {
                _val    = elms;
                Success = true;
            }
            
            private GetAll()
            {
                Success = false;
            }
            
            public static GetAll Ok(IEnumerable<LogElement> elms) => new GetAll(elms);
            public static GetAll Bad() => new GetAll();
        }
        
        public sealed class GetById
        {
            private LogElement? _val;
            
            public bool Success { get; set; }
            public LogElement Value => Success && !(_val is null) ? _val : throw new InvalidOperationException();
            
            private GetById(LogElement elm)
            {
                _val    = elm;
                Success = true;
            }
            
            private GetById()
            {
                Success = false;
            }
            
            public static GetById Ok(LogElement elm) => new GetById(elm);
            public static GetById NotFound() => new GetById();
        }
        
        public sealed class GetBySearch
        {
            private IEnumerable<LogElement>? _val;
            private string? error_msg;
            
            public bool Success { get; private set; }
            public IEnumerable<LogElement> Value => Success && !(_val is null) ? _val : throw new InvalidOperationException();
            public string Error => !Success && !(error_msg is null) ? error_msg : throw new InvalidOperationException();
            
            private GetBySearch(IEnumerable<LogElement> elms)
            {
                _val    = elms;
                Success = true;
            }
            
            private GetBySearch(string error)
            {
                error_msg = error;
                Success   = false;
            }
            
            private GetBySearch() : this("Bad Request")
            {
                
            }
            
            public static GetBySearch Ok(IEnumerable<LogElement> elms) => new GetBySearch(elms);
            public static GetBySearch InvalidField(string name) => new GetBySearch(name);
            public static GetBySearch InvalidTerm(string name) => new GetBySearch(name);
            public static GetBySearch Bad() => new GetBySearch();
        }
        
        public sealed class Put
        {
            public ResultKind Result { get; private set; }
            public bool Success => Result == ResultKind.Ok;
            
            private Put(ResultKind kind)
            {
                Result = kind;
            }
            
            public enum ResultKind
            {
                None,
                Ok,
                BadRequest,
                NotFound,
                NoContent,
                Unauthorized,
            }
            
            public static Put Unauthorized() => new Put(ResultKind.Unauthorized);
        }
        
        public sealed class Post
        {
            private LogElement? _val;
            private Exception? _error;
            
            public bool Success { get; private set; }
            public LogElement Value => Success && !(_val is null) ? _val : throw new InvalidOperationException();
            public Exception Error => !Success && !(_error is null) ? _error : throw new InvalidOperationException();
            
            private Post(LogElement elm)
            {
                _val    = elm;
                Success = true;
            }
            
            private Post(Exception err)
            {
                _error  = err;
                Success = false;
            }
            
            public static Post Ok(LogElement elm) => new Post(elm);
            public static Post Bad(Exception err) => new Post(err);
        }
        
        public sealed class Delete
        {
            public ResultKind Result { get; private set; }
            public bool Success => Result == ResultKind.Ok;
            
            private Delete(ResultKind kind)
            {
                Result = kind;
            }
            
            public static Delete Ok() => new Delete(ResultKind.Ok);
            public static Delete NotFound() => new Delete(ResultKind.NotFound);
            public static Delete Unauthorized() => new Delete(ResultKind.Unauthorized);
            
            public enum ResultKind
            {
                None,
                Ok,
                NotFound,
                Unauthorized,
            }
        }
    }
}
