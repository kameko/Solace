
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
        public bool Closed => Subscriber.Closed && Producer.Closed;
        
        private CommunicationContract(CommunicationToken subscriber, CommunicationToken producer)
        {
            Subscriber = subscriber;
            Producer   = producer;
        }
        
        public static CommunicationContract Create(string subscriber_name, string producer_name)
        {
            var c1 = Channel.CreateUnbounded<Message>();
            var c2 = Channel.CreateUnbounded<Message>();
            
            return CreatorHelper(subscriber_name, producer_name, c1, c2);
        }
        
        public static CommunicationContract Create(string subscriber_name, string producer_name, int capacity)
        {
            if (capacity > 0)
            {
                var c1 = Channel.CreateBounded<Message>(capacity);
                var c2 = Channel.CreateBounded<Message>(capacity);
                
                return CreatorHelper(subscriber_name, producer_name, c1, c2);
            }
            else
            {
                return Create(subscriber_name, producer_name);
            }
        }
        
        private static CommunicationContract CreatorHelper(string subscriber_name, string producer_name, Channel<Message> c1, Channel<Message> c2)
        {
            var t1 = new CommunicationToken(subscriber_name, c1, c2);
            var t2 = new CommunicationToken(producer_name  , c2, c1);
            
            var cc = new CommunicationContract(t1, t2);
            
            t1.Contract = cc;
            t2.Contract = cc;
            
            t1.Other = t2;
            t2.Other = t1;
            
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
        
        public override string ToString()
        {
            string relationship = string.Empty;
            
            if (Closed)
            {
                relationship = "<-x->";
            }
            else if (Subscriber.Closed)
            {
                relationship = "<--";
            }
            else if (Producer.Closed)
            {
                relationship = "-->";
            }
            else
            {
                relationship = "<--->";
            }
            
            return $"{Subscriber.Name} {relationship} {Producer.Name}";
        }
    }
}
