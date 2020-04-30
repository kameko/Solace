
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;
    using Logging;
    using Entities;
    
    public abstract class EntityFrameworkControllerService<TService, TKey, T, TTerm, TSource>
        : IControllerService<TKey, T>
        , IControllerSource<TKey, T, TSource>
        where T : IHasId<TKey>
        where TSource : DbContext
    {
        protected FileInfo SourcePath { get; set; }
        protected ILogger Log { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        
        public int GetLimit { get; protected set; }
        
        public EntityFrameworkControllerService(ILogger<TService> logger, IConfiguration config)
        {
            SourcePath             = null!;
            
            Log           = logger;
            Configuration = config;
        }
        
        protected void Reconfigure(string service_name)
        {
            SourcePath = new FileInfo(Configuration[$"{service_name}:Path"]);
            if (!int.TryParse(Configuration[$"Networking:GetLimit"], out var get_limit))
            {
                get_limit = 100;
            }
            GetLimit = get_limit;
        }
        
        public virtual Task<ControllerResult.GetOne<T>> GetOne()
        {
            Log.Warning("{Method} unsupported.", nameof(GetOne));
            return GetOneUnsupported();
        }
        
        public virtual Task<ControllerResult.GetAll<T>> GetAll()
        {
            Log.Warning("{Method} unsupported.", nameof(GetAll));
            return GetAllUnsupported();
        }
        
        public virtual Task<ControllerResult.GetById<T>> GetById(TKey id)
        {
            Log.Warning("{Method} unsupported.", nameof(GetById));
            return GetByIdUnsupported();
        }
        
        public virtual Task<ControllerResult.Put> Put(TKey id, T value)
        {
            Log.Warning("{Method} unsupported.", nameof(Put));
            return PutUnsupported();
        }
        
        public virtual Task<ControllerResult.Post<T>> Post(T value)
        {
            Log.Warning("{Method} unsupported.", nameof(Post));
            return PostUnsupported();
        }
        
        public virtual Task<ControllerResult.DeleteAll> DeleteAll()
        {
            Log.Warning("{Method} unsupported.", nameof(DeleteAll));
            return DeleteAllUnsupported();
        }
        
        public virtual Task<ControllerResult.DeleteById> DeleteById(TKey id)
        {
            Log.Warning("{Method} unsupported.", nameof(DeleteById));
            return DeleteByIdUnsupported();
        }
        
        // --- Default Options --- //
        
        protected Task<ControllerResult.GetOne<T>> DefaultGetOne(Func<TSource, T> producer) =>
            DefaultGetOne(producer, SourcePath, ContextFactory, SeedFactory);
        
        protected async Task<ControllerResult.GetOne<T>> DefaultGetOne(
            Func<TSource, T> producer,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(GetOne));
            
            await CreateDatabaseIfNotExist(source, source_factory, seed_factory);
            
            using (var context = source_factory.Invoke())
            {
                var elm = producer.Invoke(context);
                Log.ExitMethod(nameof(GetOne));
                return ControllerResult.GetOne<T>.Ok(elm);
            }
        }
        
        protected Task<ControllerResult.GetAll<T>> DefaultGetAll(Func<TSource, IEnumerable<T>> producer) =>
            DefaultGetAll(producer, SourcePath, ContextFactory, SeedFactory);
        
        protected async Task<ControllerResult.GetAll<T>> DefaultGetAll(
            Func<TSource, IEnumerable<T>> producer,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(GetAll));
            
            await CreateDatabaseIfNotExist(source, source_factory, seed_factory);
            
            var context = source_factory.Invoke();
            var db_elms = producer.Invoke(context);
            Log.ExitMethod(nameof(GetAll));
            return ControllerResult.GetAll<T>.Ok(db_elms);
        }
        
        protected Task<ControllerResult.GetById<T>> DefaultGetById(TKey id, Func<TSource, T> producer) =>
            DefaultGetById(id, producer, SourcePath, ContextFactory, SeedFactory);
        
        protected async Task<ControllerResult.GetById<T>> DefaultGetById(
            TKey id,
            Func<TSource, T> producer,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(GetById));
            
            await CreateDatabaseIfNotExist(source, source_factory, seed_factory);
            
            var context = source_factory.Invoke();
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
        
        protected Task<ControllerResult.Put> DefaultPut(TKey id, T value, Func<TSource, bool> updater) =>
            DefaultPut(id, value, updater, SourcePath, ContextFactory, SeedFactory);
        
        protected async Task<ControllerResult.Put> DefaultPut(
            TKey id,
            T value,
            Func<TSource, bool> updater,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(Put));
            
            await CreateDatabaseIfNotExist(source, source_factory, seed_factory);
            
            var context = source_factory.Invoke();
            var success = updater.Invoke(context);
            if (success)
            {
                await context.SaveChangesAsync();
                Log.ExitMethod(nameof(Put));
                return ControllerResult.Put.NoContent();
            }
            else
            {
                Log.ExitMethod(nameof(Put));
                return ControllerResult.Put.BadRequest();
            }
        }
        
        protected Task<ControllerResult.Post<T>> DefaultPost(Func<TSource, T> producer, Action<TSource> joiner) =>
            DefaultPost(producer, joiner, SourcePath, ContextFactory, SeedFactory);
        
        protected async Task<ControllerResult.Post<T>> DefaultPost(
            Func<TSource, T> producer,
            Action<TSource> joiner,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(Post));
            
            await CreateDatabaseIfNotExist(source, source_factory, seed_factory);
            
            var context = source_factory.Invoke();
            var elm = producer.Invoke(context);
            if (elm is null)
            {
                joiner.Invoke(context);
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
        
        protected Task<ControllerResult.DeleteAll> DefaultDeleteAll(Func<TSource, ControllerResult.DeleteAll> remover) =>
            DefaultDeleteAll(remover, SourcePath, ContextFactory, SeedFactory);
        
        protected async Task<ControllerResult.DeleteAll> DefaultDeleteAll(
            Func<TSource, ControllerResult.DeleteAll> remover,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(DeleteAll));
            
            await CreateDatabaseIfNotExist(source, source_factory, seed_factory);
            
            var context = source_factory.Invoke();
            var result = remover.Invoke(context);
            await context.SaveChangesAsync();
            Log.ExitMethod(nameof(DeleteAll));
            return result;
        }
        
        protected Task<ControllerResult.DeleteById> DefaultDeleteById(TKey id, Func<TSource, ControllerResult.DeleteById> remover) =>
            DefaultDeleteById(id, remover, SourcePath, ContextFactory, SeedFactory);
        
        protected async Task<ControllerResult.DeleteById> DefaultDeleteById(
            TKey id,
            Func<TSource, ControllerResult.DeleteById> remover,
            FileInfo source,
            Func<TSource> source_factory,
            Action<TSource> seed_factory)
        {
            Log.EnterMethod(nameof(DeleteById));
            
            await CreateDatabaseIfNotExist(source, source_factory, seed_factory);
            
            var context = source_factory.Invoke();
            var result = remover.Invoke(context);
            await context.SaveChangesAsync();
            Log.ExitMethod(nameof(DeleteById));
            return result;
        }
        
        // --- Utilities --- //
        
        protected virtual TSource ContextFactory()
        {
            throw new NotImplementedException();
        }
        
        protected virtual void SeedFactory(TSource context)
        {
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
                
                var context = source_factory.Invoke();
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                seed_factory.Invoke(context);
                await context.SaveChangesAsync();
                
                Log.Information($"Created system log database at {source.FullName}.");
                
                Log.ExitMethod(nameof(CreateDatabaseIfNotExist));
            }
        }
        
        protected Task<ControllerResult.GetOne<T>> GetOneUnsupported() => Task.FromResult(ControllerResult.GetOne<T>.Unsupported());
        protected Task<ControllerResult.GetAll<T>> GetAllUnsupported() => Task.FromResult(ControllerResult.GetAll<T>.Unsupported());
        protected Task<ControllerResult.GetById<T>> GetByIdUnsupported() => Task.FromResult(ControllerResult.GetById<T>.Unsupported());
        protected Task<ControllerResult.Put> PutUnsupported() => Task.FromResult(ControllerResult.Put.Unsupported());
        protected Task<ControllerResult.Post<T>> PostUnsupported() => Task.FromResult(ControllerResult.Post<T>.Unsupported());
        protected Task<ControllerResult.DeleteAll> DeleteAllUnsupported() => Task.FromResult(ControllerResult.DeleteAll.Unsupported());
        protected Task<ControllerResult.DeleteById> DeleteByIdUnsupported() => Task.FromResult(ControllerResult.DeleteById.Unsupported());
    }
}
