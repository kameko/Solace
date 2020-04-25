
namespace Caesura.Solace.Foundation
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    
    public class LifetimeEventsHostedService : IHostedService
    {
        public static event Action OnStarted  = delegate { };
        public static event Action OnStopping = delegate { };
        public static event Action OnStopped  = delegate { };
        public static CancellationToken Token => tokenSource.Token;
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        private readonly IHostApplicationLifetime _appLifetime;
        
        public LifetimeEventsHostedService(IHostApplicationLifetime appLifetime)
        {
            _appLifetime = appLifetime;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(CallOnStarted);
            _appLifetime.ApplicationStopping.Register(CallOnStopping);
            _appLifetime.ApplicationStopped.Register(CallOnStopped);
            
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        
        private void CallOnStarted()
        {
            OnStarted.Invoke();
        }
        
        private void CallOnStopping()
        {
            tokenSource.Cancel();
            OnStopping.Invoke();
        }
        
        private void CallOnStopped()
        {
            OnStopped.Invoke();
        }
    }
}
