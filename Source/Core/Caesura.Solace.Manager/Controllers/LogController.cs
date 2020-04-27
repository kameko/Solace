
namespace Caesura.Solace.Manager.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Entities.Core;
    using Interfaces;
    
    [ApiController]
    [Route("system/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> log;
        private readonly ILogService service;
        
        public LogController(ILogger<LogController> ilog, ILogService iservice)
        {
            log     = ilog;
            service = iservice;
            
            log.LogTrace("Created LogController instance.");
            
            service.Inject(log);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogElement>>> Get()
        {
            log.LogTrace("Entering GET request for LogElements.");
            try
            {
                var service_result = await service.Get();
                log.LogDebug("GET request for LogElements. Success: {Success}", service_result.Success);
                if (service_result.Success)
                {
                    return Ok(service_result.Value);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                log.LogError(e, string.Empty);
                throw;
            }
            finally
            {
                log.LogTrace("Exiting GET request for LogElements.");
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<LogElement>> Get(ulong id)
        {
            log.LogTrace("Entering GET request for LogElement {Id}.", id);
            try
            {
                var service_result = await service.Get(id);
                log.LogDebug("GET request for LogElement {Id}, Success: {Success}.", id, service_result.Success);
                if (service_result.Success)
                {
                    return service_result.Value;
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                log.LogError(e, string.Empty);
                throw;
            }
            finally
            {
                log.LogTrace("Exiting GET request for LogElement {Id}.", id);
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(ulong id, LogElement element)
        {
            log.LogTrace("Entering PUT request for LogElement {Id} {LogElement}.", id, element);
            try
            {
                var service_result = await service.Put(id, element);
                log.LogDebug("PUT request for LogElement {Id}, Result: {Result}.", id, service_result.Result);
                return service_result.Result switch
                {
                    LogServiceResult.Put.ResultKind.Ok           => NoContent(),
                    LogServiceResult.Put.ResultKind.NoContent    => NoContent(),
                    LogServiceResult.Put.ResultKind.BadRequest   => BadRequest(),
                    LogServiceResult.Put.ResultKind.NotFound     => NotFound(),
                    LogServiceResult.Put.ResultKind.Unauthorized => Unauthorized(),
                    
                    _ => BadRequest()
                };
            }
            catch (Exception e)
            {
                log.LogError(e, string.Empty);
                throw;
            }
            finally
            {
                log.LogTrace("Exiting PUT request for LogElement {Id} {LogElement}.", id, element);
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<LogElement>> Post(LogElement element)
        {
            log.LogTrace("Entering POST request for LogElement {LogElement}.", element);
            try
            {
                var service_result = await service.Post(element);
                log.LogDebug("POST request for LogElement {LogElement}, Success: {Success}.", element, service_result.Success);
                if (service_result.Success)
                {
                    return CreatedAtAction(
                            nameof(Get),
                            new { id = element.Id },
                            service_result.Value
                        );
                }
                else
                {
                    return BadRequest(service_result.Error);
                }
            }
            catch (Exception e)
            {
                log.LogError(e, string.Empty);
                throw;
            }
            finally
            {
                log.LogTrace("Exiting POST request for LogElement {LogElement}.", element);
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(ulong id)
        {
            log.LogTrace("Entering DELETE request for LogElement {Id}.", id);
            try
            {
                var service_result = await service.Delete(id);
                log.LogDebug("DELETE request for LogElement {Id}, Result: {Result}.", id, service_result.Result);
                return service_result.Result switch
                {
                    LogServiceResult.Delete.ResultKind.Ok           => NoContent(),
                    LogServiceResult.Delete.ResultKind.NotFound     => NotFound(),
                    LogServiceResult.Delete.ResultKind.Unauthorized => Unauthorized(),
                    
                    _ => BadRequest()
                };
            }
            catch (Exception e)
            {
                log.LogError(e, string.Empty);
                throw;
            }
            finally
            {
                log.LogTrace("Exiting DELETE request for LogElement {Id}.", id);
            }
        }
    }
}
