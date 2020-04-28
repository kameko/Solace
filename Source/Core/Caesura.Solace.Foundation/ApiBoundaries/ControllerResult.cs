
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Collections.Generic;
    
    public static class ControllerResult
    {
        public enum Result
        {
            None         = 0,
            Ok           = 1,
            Threw        = 2,
            BadRequest   = 3,
            NotFound     = 4,
            NoContent    = 5,
            Unauthorized = 6,
            Unspported   = 7,
        }
        
        public class BaseResponse<T>
        {
            protected T _value { get; set; }
            protected Exception? _exception { get; set; }
            protected string _bad_request_message { get; set; } = string.Empty;
            
            public Result Result { get; protected set; }
            public virtual bool Success => Result == Result.Ok;
            public bool HasValue => !(_value is null);
            public bool Threw => Result == Result.Threw && !(Exception is null);
            public T Value => Success && HasValue ? _value : throw new InvalidOperationException();
            public Exception Exception => (Threw ? _exception : throw new InvalidOperationException())!;
            public string BadRequestMessage => _bad_request_message;
            
            internal BaseResponse()
            {
                _value = default!;
            }
            
            internal BaseResponse(Result result) : this()
            {
                Result = result;
            }
            
            internal BaseResponse(Result result, string message) : this()
            {
                Result = result;
                if (result == Result.BadRequest)
                {
                    _bad_request_message = message;
                }
            }
            
            internal BaseResponse(T value) : this()
            {
                _value = value;
                Result = HasValue ? Result.Ok : Result.NotFound;
            }
            
            internal BaseResponse(Exception exception) : this()
            {
                _exception = exception;
                Result     = Result.Threw;
            }
            
            public override string ToString()
            {
                // TODO:
                return $"{Result}";
            }
        }
        
        public sealed class GetAll<T> : BaseResponse<IEnumerable<T>>
        {
            private GetAll() : base() { }
            private GetAll(Result result) : base(result) { }
            private GetAll(IEnumerable<T> value) : base(value) { }
            private GetAll(Exception exception) : base(exception) { }
            
            public static GetAll<T> Ok(IEnumerable<T> value) => new GetAll<T>(value);
            public static GetAll<T> NotFound() => new GetAll<T>(Result.NotFound);
            public static GetAll<T> Unauthorized() => new GetAll<T>(Result.Unauthorized);
            public static GetAll<T> Unsupported() => new GetAll<T>(Result.Unspported);
            public static GetAll<T> Error(Exception exception) => new GetAll<T>(exception);
        }
        
        public sealed class GetById<T> : BaseResponse<T>
        {
            private GetById() : base() { }
            private GetById(Result result) : base(result) { }
            private GetById(T value) : base(value) { }
            private GetById(Exception exception) : base(exception) { }
            
            public static GetById<T> Ok(T value) => new GetById<T>(value);
            public static GetById<T> NotFound() => new GetById<T>(Result.NotFound);
            public static GetById<T> Unauthorized() => new GetById<T>(Result.Unauthorized);
            public static GetById<T> Unsupported() => new GetById<T>(Result.Unspported);
            public static GetById<T> Error(Exception exception) => new GetById<T>(exception);
        }
        
        public sealed class GetBySearch<T> : BaseResponse<IEnumerable<T>>
        {
            private GetBySearch() : base() { }
            private GetBySearch(Result result) : base(result) { }
            private GetBySearch(IEnumerable<T> value) : base(value) { }
            private GetBySearch(Exception exception) : base(exception) { }
            
            public static GetBySearch<T> Ok(IEnumerable<T> value) => new GetBySearch<T>(value);
            public static GetBySearch<T> NotFound() => new GetBySearch<T>(Result.NotFound);
            public static GetBySearch<T> Unauthorized() => new GetBySearch<T>(Result.Unauthorized);
            public static GetBySearch<T> Unsupported() => new GetBySearch<T>(Result.Unspported);
            public static GetBySearch<T> Error(Exception exception) => new GetBySearch<T>(exception);
        }
        
        public sealed class Put : BaseResponse<object>
        {
            public override bool Success => Result == Result.NoContent;
            
            private Put() : base() { }
            private Put(Result result) : base(result) { }
            private Put(Result result, string message) : base(result, message) { }
            private Put(Exception exception) : base(exception) { }
            
            public static Put Ok() => new Put(Result.NoContent);
            public static Put NoContent() => new Put(Result.NoContent);
            public static Put NotFound() => new Put(Result.NotFound);
            public static Put BadRequest() => new Put(Result.BadRequest);
            public static Put BadRequest(string message) => new Put(Result.BadRequest, message);
            public static Put Unauthorized() => new Put(Result.Unauthorized);
            public static Put Unsupported() => new Put(Result.Unspported);
            public static Put Error(Exception exception) => new Put(exception);
        }
        
        public sealed class Post<T> : BaseResponse<T>
        {
            private Post() : base() { }
            private Post(Result result) : base(result) { }
            private Post(Result result, string message) : base(result, message) { }
            private Post(T value) : base(value) { }
            private Post(Exception exception) : base(exception) { }
            
            public static Post<T> Ok(T value) => new Post<T>(value);
            public static Post<T> NotFound() => new Post<T>(Result.NotFound);
            public static Post<T> BadRequest() => new Post<T>(Result.BadRequest);
            public static Post<T> BadRequest(string message) => new Post<T>(Result.BadRequest, message);
            public static Post<T> Unauthorized() => new Post<T>(Result.Unauthorized);
            public static Post<T> Unsupported() => new Post<T>(Result.Unspported);
            public static Post<T> Error(Exception exception) => new Post<T>(exception);
        }
        
        public sealed class Delete : BaseResponse<object>
        {
            private Delete() : base() { }
            private Delete(Result result) : base(result) { }
            private Delete(Exception exception) : base(exception) { }
            
            public static Delete Ok() => new Delete(Result.Ok);
            public static Delete NotFound() => new Delete(Result.NotFound);
            public static Delete Unauthorized() => new Delete(Result.Unauthorized);
            public static Delete Unsupported() => new Delete(Result.Unspported);
        }
    }
}
