
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Channels;
    
    public class CommunicationContract : IDisposable, IAsyncDisposable
    {
        public CommunicationToken Subscriber { get; private set; }
        public CommunicationToken Producer { get; private set; }
        public bool Disposed => Subscriber.Disposed && Producer.Disposed;
        
        public CommunicationContract(CommunicationToken subscriber, CommunicationToken producer)
        {
            Subscriber = subscriber;
            Producer   = producer;
        }
        
        public static CommunicationContract Create(string name1, string name2)
        {
            var c1 = Channel.CreateUnbounded<Message>();
            var c2 = Channel.CreateUnbounded<Message>();
            
            var t1 = new CommunicationToken(name1, c1, c2);
            var t2 = new CommunicationToken(name2, c2, c1);
            
            var cc = new CommunicationContract(t1, t2);
            
            return cc;
        }
        
        public void Dispose()
        {
            Subscriber.Dispose();
            Producer.Dispose();
        }
        
        public async ValueTask DisposeAsync()
        {
            await Subscriber.DisposeAsync();
            await Producer.DisposeAsync();
        }
    }
}
