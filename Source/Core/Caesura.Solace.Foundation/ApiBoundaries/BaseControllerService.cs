
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using Logging;
    
    public abstract class BaseControllerService<TService, TKey, T, TTerm, TSource>
        : IControllerService<TKey, T, TSource>
        , IControllerSearchableService<T, TTerm, TSource>
        where TSource : DbContext
    {
        protected FileInfo SourcePath { get; set; }
        protected string SourceConnectionString { get; set; }
        protected ILogger Log { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        
        public int GetLimit { get; protected set; }
        
        public BaseControllerService(ILogger<TService> logger, IConfiguration config)
        {
            SourcePath             = null!;
            SourceConnectionString = null!;
            
            Log           = logger;
            Configuration = config;
        }
        
        protected void Reconfigure(string db_path_elm, string db_con_string_elm, string get_limit_elm)
        {
            SourcePath             = new FileInfo(Configuration["LogService:DatabasePath"]);
            SourceConnectionString = Configuration["LogService:ConnectionString"].Replace("{DatabasePath}", SourcePath.FullName);
            if (!int.TryParse(Configuration["LogService:GetLimit"], out var get_limit))
            {
                get_limit = 100;
            }
            GetLimit = get_limit;
        }
        
        public virtual Task<ControllerResult.GetAll<T>> GetAll()
        {
            return GetAllUnsupported();
        }
        
        public virtual Task<ControllerResult.GetById<T>> GetById(TKey id)
        {
            return GetByIdUnsupported();
        }
        
        public virtual Task<ControllerResult.GetBySearch<T>> GetBySearch(string field, TTerm term)
        {
            return GetBySearchUnsupported();
        }
        
        public virtual Task<ControllerResult.Put> Put(TKey id, T value)
        {
            return PutUnsupported();
        }
        
        public virtual Task<ControllerResult.Post<T>> Post(T value)
        {
            return PostUnsupported();
        }
        
        public virtual Task<ControllerResult.Delete> Delete(TKey id)
        {
            return DeleteUnsupported();
        }
        
        protected async Task<ControllerResult.GetAll<T>> DefaultGetAll(
            Func<TSource, IEnumerable<T>> producer,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(GetAll));
            
            await CreateDatabaseIfNotExist(SourcePath, source_factory, seed_factory);
            
            var elms = new List<T>();
            using (var context = source_factory.Invoke())
            {
                var db_elms = producer.Invoke(context);
                // required or the context will throw when we try to pass
                // db_elms to the caller.
                elms.AddRange(db_elms);
            }
            
            Log.ExitMethod(nameof(GetAll));
            return ControllerResult.GetAll<T>.Ok(elms);
        }
        
        protected async Task<ControllerResult.GetById<T>> DefaultGetById(
            TKey id,
            Func<TSource, T> producer,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(GetById));
            
            await CreateDatabaseIfNotExist(SourcePath, source_factory, seed_factory);
            
            using (var context = source_factory.Invoke())
            {
                var elm = producer.Invoke(context);
                if (elm is null)
                {
                    var bad = ControllerResult.GetById<T>.NotFound();
                    Log.ExitMethod(nameof(GetById));
                    return bad;
                }
                else
                {
                    var ok = ControllerResult.GetById<T>.Ok(elm);
                    Log.ExitMethod(nameof(GetById));
                    return ok;
                }
            }
        }
        
        protected async Task<ControllerResult.GetBySearch<T>> DefaultGetBySearch(
            string field,
            TTerm term,
            Func<TSource, IEnumerable<T>> producer,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(GetBySearch));
            
            await CreateDatabaseIfNotExist(SourcePath, source_factory, seed_factory);
            
            // TODO: possible methods to implement this:
            // - iterate over property names (call seed_factory for an instance to iterate over)
            // - list of callbacks for properties and how to compare them
            
            Log.ExitMethod(nameof(GetBySearch));
            
            throw new NotImplementedException("Currently can't think of a good way to do a generic search.");
        }
        
        protected async Task<ControllerResult.Put> DefaultPut(
            TKey id,
            T value,
            Func<TSource, T> producer,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(Put));
            
            await CreateDatabaseIfNotExist(SourcePath, source_factory, seed_factory);
            
            // TODO:
            
            Log.ExitMethod(nameof(Put));
            
            throw new NotImplementedException();
        }
        
        protected async Task<ControllerResult.Post<T>> DefaultPost(
            T value,
            Func<TSource, T> producer,
            Action<TSource, T> joiner,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(Post));
            
            await CreateDatabaseIfNotExist(SourcePath, source_factory, seed_factory);
            
            using (var context = source_factory.Invoke())
            {
                var elm = producer.Invoke(context);
                if (elm is null)
                {
                    joiner.Invoke(context, value);
                    await context.SaveChangesAsync();
                    elm = producer.Invoke(context);
                    if (elm is null)
                    {
                        Log.ExitMethod(nameof(Post));
                        return ControllerResult.Post<T>.BadRequest("Item was added but not found.");
                    }
                    else
                    {
                        Log.ExitMethod(nameof(Post));
                        return ControllerResult.Post<T>.Ok(elm);
                    }
                }
                else
                {
                    Log.ExitMethod(nameof(Post));
                    return ControllerResult.Post<T>.BadRequest("Item already exists.");
                }
            }
        }
        
        protected async Task<ControllerResult.Delete> DefaultDelete(
            TKey id,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory,
            Func<TSource, T> producer)
        {
            Log.EnterMethod(nameof(Delete));
            
            await CreateDatabaseIfNotExist(SourcePath, source_factory, seed_factory);
            
            // TODO:
            
            Log.ExitMethod(nameof(Delete));
            
            throw new NotImplementedException();
        }
        
        protected virtual async Task CreateDatabaseIfNotExist(FileInfo source, Func<TSource> source_factory, Action<TSource> seed_factory)
        {
            if (!File.Exists(source.FullName))
            {
                Log.EnterMethod(nameof(CreateDatabaseIfNotExist));
                
                Log.Information($"Database file not found at {source.FullName}. Creating...");
                
                var dir = source.Directory?.FullName ?? string.Empty;
                if (string.IsNullOrEmpty(dir))
                {
                    throw new ArgumentException($"{source.FullName} is not a valid path.");
                }
                else if (!Directory.Exists(dir))
                {
                    Log.Information($"Database directory \"{dir}\" not found. Creating...");
                    Directory.CreateDirectory(dir);
                }
                
                using (var context = source_factory.Invoke())
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.EnsureCreatedAsync();
                    seed_factory.Invoke(context);
                    await context.SaveChangesAsync();
                }
                
                Log.Information($"Created system log database at {source.FullName}.");
                
                Log.ExitMethod(nameof(CreateDatabaseIfNotExist));
            }
        }
        
        protected Task<ControllerResult.GetAll<T>> GetAllUnsupported() => Task.FromResult(ControllerResult.GetAll<T>.Unsupported());
        protected Task<ControllerResult.GetById<T>> GetByIdUnsupported() => Task.FromResult(ControllerResult.GetById<T>.Unsupported());
        protected Task<ControllerResult.GetBySearch<T>> GetBySearchUnsupported() => Task.FromResult(ControllerResult.GetBySearch<T>.Unsupported());
        protected Task<ControllerResult.Put> PutUnsupported() => Task.FromResult(ControllerResult.Put.Unsupported());
        protected Task<ControllerResult.Post<T>> PostUnsupported() => Task.FromResult(ControllerResult.Post<T>.Unsupported());
        protected Task<ControllerResult.Delete> DeleteUnsupported() => Task.FromResult(ControllerResult.Delete.Unsupported());
    }
}
