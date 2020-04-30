
namespace Caesura.Solace.Foundation.ApiBoundaries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Foundation.Logging;
    using Entities;
    
    public abstract class BaseServiceController<TController, T, TService>
        : ControllerBase
        where T : IHasId<ulong>
        where TService : IControllerService<ulong, T>
    {
        private string t_name;
        protected TService Service { get; private set; }
        protected ILogger Log { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        
        public BaseServiceController(TService service, ILogger<TController> logger, IConfiguration configuration)
        {
            t_name = typeof(T).Name;
            
            Service       = service;
            Log           = logger;
            Configuration = configuration;
        }
        
        // TODO: GetOneDefault()
        
        public virtual async Task<ActionResult<IEnumerable<T>>> GetAllDefault()
        {
            Log.EnterMethod(nameof(GetAllDefault));
            try
            {
                var service_result = await Service.GetAll();
                if (service_result.Success)
                {
                    var val = service_result.Value;
                    Log.Debug($"Successful GET request for {t_name}. Returned {{num}} item(s).", val.Count());
                    return Ok(val);
                }
                else
                {
                    Log.Debug($"Unsuccessful GET request for {t_name}.");
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                throw;
            }
            finally
            {
                Log.ExitMethod(nameof(GetAllDefault));
            }
        }
        
        public virtual async Task<ActionResult<T>> GetByIdDefault(ulong id)
        {
            Log.EnterMethod(nameof(GetByIdDefault), "for Id {Id}.", id);
            try
            {
                var service_result = await Service.GetById(id);
                if (service_result.Success)
                {
                    var val = service_result.Value!;
                    Log.Debug($"Successful GET request for {t_name} {{Id}}. Returned item: {{{t_name}}}.", id, val);
                    return val;
                }
                else
                {
                    Log.Debug($"Unsuccessful GET request for {t_name} {{Id}}.", id);
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                throw;
            }
            finally
            {
                Log.ExitMethod(nameof(GetByIdDefault), "for Id {Id}.", id);
            }
        }
        
        public virtual async Task<IActionResult> PutDefault(ulong id, T element)
        {
            Log.EnterMethod(nameof(PutDefault), $"for Id {{Id}} and {t_name} {{{t_name}}}.", id, element!);
            try
            {
                var service_result = await Service.Put(id, element);
                Log.Debug("PUT request for LogElement {Id}, Result: {Result}.", id, service_result.Result);
                return service_result.Result switch
                {
                    ControllerResult.Result.NoContent    => NoContent(),
                    ControllerResult.Result.BadRequest   => BadRequest(),
                    ControllerResult.Result.NotFound     => NotFound(),
                    ControllerResult.Result.Unauthorized => Unauthorized(),
                    ControllerResult.Result.Unsupported  => BadRequest(),
                    
                    _ => BadRequest()
                };
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                throw;
            }
            finally
            {
                Log.ExitMethod(nameof(PutDefault), $"for Id {{Id}} and LogElement {t_name} {{{t_name}}}.", id, element!);
            }
        }
        
        public virtual async Task<ActionResult<T>> PostDefault(T element)
        {
            Log.EnterMethod(nameof(PostDefault), $"for LogElement {t_name} {{{t_name}}}.", element!);
            try
            {
                var service_result = await Service.Post(element);
                if (service_result.Success)
                {
                    var val = service_result.Value!;
                    Log.Debug($"Successful POST request for {t_name} {{{t_name}1}}. Returned item: {{{t_name}2}}.", element!, val);
                    return CreatedAtAction(
                            nameof(GetByIdDefault),
                            new { id = element.Id },
                            val
                        );
                }
                else
                {
                    Log.Debug($"Unuccessful POST request for {t_name}: {{{t_name}}}.", element!);
                    return BadRequest(service_result.BadRequestMessage);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                throw;
            }
            finally
            {
                Log.ExitMethod(nameof(PostDefault), $"for {t_name} {{{t_name}}}.", element!);
            }
        }
        
        public virtual async Task<IActionResult> DeleteAllDefault()
        {
            Log.EnterMethod(nameof(DeleteAllDefault));
            try
            {
                var service_result = await Service.DeleteAll();
                Log.Debug($"DELETE request for {t_name}, Result: {{Result}}.", service_result.Result);
                return service_result.Result switch
                {
                    ControllerResult.Result.Ok           => Ok(),
                    ControllerResult.Result.NoContent    => NoContent(),
                    ControllerResult.Result.NotFound     => NotFound(),
                    ControllerResult.Result.Unauthorized => Unauthorized(),
                    
                    _ => BadRequest()
                };
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                throw;
            }
            finally
            {
                Log.ExitMethod(nameof(DeleteAllDefault));
            }
        }
        
        public virtual async Task<IActionResult> DeleteByIdDefault(ulong id)
        {
            Log.EnterMethod(nameof(DeleteByIdDefault), "for Id {Id}.", id);
            try
            {
                var service_result = await Service.DeleteById(id);
                Log.Debug($"DELETE request for {t_name} {{Id}}, Result: {{Result}}.", id, service_result.Result);
                return service_result.Result switch
                {
                    ControllerResult.Result.Ok           => Ok(),
                    ControllerResult.Result.NoContent    => NoContent(),
                    ControllerResult.Result.NotFound     => NotFound(),
                    ControllerResult.Result.Unauthorized => Unauthorized(),
                    
                    _ => BadRequest()
                };
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                throw;
            }
            finally
            {
                Log.ExitMethod(nameof(DeleteByIdDefault), "for Id {Id}.", id);
            }
        }
    }
}
