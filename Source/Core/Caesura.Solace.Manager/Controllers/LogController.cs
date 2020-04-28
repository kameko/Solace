
namespace Caesura.Solace.Manager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Foundation.Logging;
    using Foundation.ApiBoundaries;
    using Entities.Core;
    using Interfaces;
    
    // TODO: see if we can make some kind of layer between an IControllerService
    // and a Controller so we can avoid writing all this.
    
    [ApiController]
    [Route("system/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> Log;
        private readonly IConfiguration Configuration;
        private readonly ILogService Service;
        
        public LogController(ILogger<LogController> ilog, IConfiguration configuration, ILogService iservice)
        {
            Log            = ilog;
            Service        = iservice;
            Configuration  = configuration;
            
            Log.InstanceAbreaction();
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogElement>>> Get()
        {
            Log.EnterMethod(nameof(Get));
            try
            {
                var service_result = await Service.GetAll();
                if (service_result.Success)
                {
                    var val = service_result.Value;
                    Log.Debug("Successful GET request for LogElements. Returned {num} item(s).", val.Count());
                    return Ok(val);
                }
                else
                {
                    Log.Debug("Unsuccessful GET request for LogElements.");
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
                Log.ExitMethod(nameof(Get));
            }
        }
        
        [HttpGet("search/{field}/{term}")]
        public async Task<ActionResult<LogElement>> Get(string field, string term)
        {
            term = term.Replace("\u0022", string.Empty);
            Log.EnterMethod(nameof(Get), "with search field {field} and term {term}", field, term);
            try
            {
                var service_result = await Service.GetBySearch(field, term);
                if (service_result.Success)
                {
                    var val = service_result.Value;
                    Log.Debug(
                        "Successful GET search for LogElements with search term {field}/{term}. "
                        + "Returned {num} item(s).", field, term, val.Count());
                    return Ok(val);
                }
                else
                {
                    Log.Debug(
                        "Unsuccessful GET search for LogElements with search term "
                        + "{field}/{term}. Error: {error}", field, term, service_result.Exception
                    );
                    return BadRequest($"Invalid term: {service_result.Exception}");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, string.Empty);
                throw;
            }
            finally
            {
                Log.ExitMethod(nameof(Get), "with search field {field} and term {term}", field, term);
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<LogElement>> Get(ulong id)
        {
            Log.EnterMethod(nameof(Get), "for Id {Id}.", id);
            try
            {
                var service_result = await Service.GetById(id);
                if (service_result.Success)
                {
                    var val = service_result.Value;
                    Log.Debug("Successful GET request for LogElement {Id}. Returned item: {LogElement}.", id, val);
                    return val;
                }
                else
                {
                    Log.Debug("Unsuccessful GET request for LogElement {Id}.", id);
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
                Log.ExitMethod(nameof(Get), "for Id {Id}.", id);
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(ulong id, LogElement element)
        {
            Log.EnterMethod(nameof(Put), "for Id {Id} and LogElement {LogElement}.", id, element);
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
                Log.ExitMethod(nameof(Put), "for Id {Id} and LogElement {LogElement}.", id, element);
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<LogElement>> Post(LogElement element)
        {
            Log.EnterMethod(nameof(Post), "for LogElement {LogElement}.", element);
            try
            {
                var service_result = await Service.Post(element);
                if (service_result.Success)
                {
                    var val = service_result.Value;
                    Log.Debug("Successful POST request for LogElement: {LogElement1}. Returned item: {LogElement2}.", element, val);
                    return CreatedAtAction(
                            nameof(Get),
                            new { id = element.Id },
                            val
                        );
                }
                else
                {
                    Log.Debug("Unuccessful POST request for LogElement: {LogElement1}.", element);
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
                Log.ExitMethod(nameof(Post), "for LogElement {LogElement}.", element);
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(ulong id)
        {
            Log.EnterMethod(nameof(Delete), "for Id {Id}.", id);
            try
            {
                var service_result = await Service.DeleteById(id);
                Log.Debug("DELETE request for LogElement {Id}, Result: {Result}.", id, service_result.Result);
                return service_result.Result switch
                {
                    ControllerResult.Result.Ok           => NoContent(),
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
                Log.ExitMethod(nameof(Delete), "for Id {Id}.", id);
            }
        }
    }
}
