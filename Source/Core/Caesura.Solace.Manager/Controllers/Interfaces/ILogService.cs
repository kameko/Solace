
namespace Caesura.Solace.Manager.Controllers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Entities.Core;
    
    public interface ILogService
    {
        void Inject(ILogger ilog);
        Task<LogServiceResult.Get> Get();
        Task<LogServiceResult.GetById> Get(ulong id);
        Task<LogServiceResult.Put> Put(ulong id, LogElement element);
        Task<LogServiceResult.Post> Post(LogElement element);
        Task<LogServiceResult.Delete> Delete(ulong id);
    }
    
    public static class LogServiceResult
    {
        public class Get
        {
            private IEnumerable<LogElement>? _val;
            
            public bool Success { get; private set; }
            public IEnumerable<LogElement> Value => Success && !(_val is null) ? _val : throw new InvalidOperationException();
            
            private Get(IEnumerable<LogElement> elms)
            {
                _val    = elms;
                Success = true;
            }
            
            private Get()
            {
                Success = false;
            }
            
            public static Get Ok(IEnumerable<LogElement> elms) => new Get(elms);
            public static Get Bad() => new Get();
        }
        
        public class GetById
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
        
        public class Put
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
        
        public class Post
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
        
        public class Delete
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
