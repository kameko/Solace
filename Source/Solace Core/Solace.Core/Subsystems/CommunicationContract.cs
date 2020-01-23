
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
        
        public static CommunicationContract Create(string subscriber_name, string producer_name)
        {
            var c1 = Channel.CreateUnbounded<Message>();
            var c2 = Channel.CreateUnbounded<Message>();
            
            var t1 = new CommunicationToken(subscriber_name, c1, c2);
            var t2 = new CommunicationToken(producer_name  , c2, c1);
            
            var cc = new CommunicationContract(t1, t2);
            
            t1.Contract = cc;
            t2.Contract = cc;
            
            return cc;
        }
        
        public bool AsSubscriber(Message message)
        {
            return Subscriber.Send(message);
        }
        
        public bool AsProducer(Message message)
        {
            return Producer.Send(message);
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
