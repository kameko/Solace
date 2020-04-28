
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
            Unsupported  = 7,
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
        
        public sealed class GetOne<T> : BaseResponse<T>
        {
            private GetOne() : base() { }
            private GetOne(Result result) : base(result) { }
            private GetOne(T value) : base(value) { }
            private GetOne(Exception exception) : base(exception) { }
            
            public static GetOne<T> Ok(T value) => new GetOne<T>(value);
            public static GetOne<T> NotFound() => new GetOne<T>(Result.NotFound);
            public static GetOne<T> Unauthorized() => new GetOne<T>(Result.Unauthorized);
            public static GetOne<T> Unsupported() => new GetOne<T>(Result.Unsupported);
            public static GetOne<T> Error(Exception exception) => new GetOne<T>(exception);
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
            public static GetAll<T> Unsupported() => new GetAll<T>(Result.Unsupported);
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
            public static GetById<T> Unsupported() => new GetById<T>(Result.Unsupported);
            public static GetById<T> Error(Exception exception) => new GetById<T>(exception);
        }
        
        public sealed class GetBySearch<T> : BaseResponse<IEnumerable<T>>
        {
            private GetBySearch() : base() { }
            private GetBySearch(Result result) : base(result) { }
            private GetBySearch(Result result, string message) : base(result, message) { }
            private GetBySearch(IEnumerable<T> value) : base(value) { }
            private GetBySearch(Exception exception) : base(exception) { }
            
            public static GetBySearch<T> Ok(IEnumerable<T> value) => new GetBySearch<T>(value);
            public static GetBySearch<T> NotFound() => new GetBySearch<T>(Result.NotFound);
            public static GetBySearch<T> BadRequest(string message) => new GetBySearch<T>(Result.BadRequest, message);
            public static GetBySearch<T> Unauthorized() => new GetBySearch<T>(Result.Unauthorized);
            public static GetBySearch<T> Unsupported() => new GetBySearch<T>(Result.Unsupported);
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
            public static Put Unsupported() => new Put(Result.Unsupported);
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
            public static Post<T> Unsupported() => new Post<T>(Result.Unsupported);
            public static Post<T> Error(Exception exception) => new Post<T>(exception);
        }
        
        public sealed class DeleteAll : BaseResponse<object>
        {
            private DeleteAll() : base() { }
            private DeleteAll(Result result) : base(result) { }
            private DeleteAll(Exception exception) : base(exception) { }
            
            public static DeleteAll Ok() => new DeleteAll(Result.Ok);
            public static DeleteAll NotFound() => new DeleteAll(Result.NotFound);
            public static DeleteAll Unauthorized() => new DeleteAll(Result.Unauthorized);
            public static DeleteAll Unsupported() => new DeleteAll(Result.Unsupported);
        }
        
        public sealed class DeleteById : BaseResponse<object>
        {
            private DeleteById() : base() { }
            private DeleteById(Result result) : base(result) { }
            private DeleteById(Exception exception) : base(exception) { }
            
            public static DeleteById Ok() => new DeleteById(Result.Ok);
            public static DeleteById NotFound() => new DeleteById(Result.NotFound);
            public static DeleteById Unauthorized() => new DeleteById(Result.Unauthorized);
            public static DeleteById Unsupported() => new DeleteById(Result.Unsupported);
        }
    }
}
