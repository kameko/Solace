
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Channels;
    
    public abstract class BaseSubsystem : ISubsystem
    {
        private List<SubscriberToken> Subscribers { get; set; }
        
        public BaseSubsystem()
        {
            Subscribers = new List<SubscriberToken>();
        }
        
        protected abstract Task Execute();
        
        public abstract SubsystemExecutorOptions GetExecutorOptions();
        
        public virtual Func<Task>? GetExecutor()
        {
            return Execute;
        }
        
        public virtual ChannelReader<object> Subscribe(string name)
        {
            return Subscribe(name, 0);
        }
        
        public virtual ChannelReader<object> Subscribe(string name, int capacity)
        {
            Channel<object> channel = capacity <= 0 
                ? Channel.CreateUnbounded<object>()
                : Channel.CreateBounded<object>(capacity: capacity);
            
            var token = new SubscriberToken(name, channel);
            return channel.Reader;
        }
        
        public virtual bool Unsubscribe(string name)
        {
            var subscriber = Subscribers.Find(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            if (!(subscriber is null))
            {
                var result = Subscribers.Remove(subscriber);
                subscriber.Channel.Writer.TryComplete();
                return result;
            }
            return false;
        }
        
        public abstract void PlugInput(ChannelReader<object> reader);
        
        protected Task Publish(object item)
        {
            var task = Task.Run(() =>
            {
                foreach (var subscriber in Subscribers)
                {
                    subscriber.Writer.TryWrite(item);
                }
            });
            return task;
        }
        
        public virtual void Dispose()
        {
            foreach (var subscriber in Subscribers)
            {
                subscriber.Channel.Writer.TryComplete();
            }
            Subscribers.Clear();
        }
        
        public virtual ValueTask DisposeAsync()
        {
            var task = Task.Run(() =>
            {
                Dispose();
            });
            return new ValueTask(task);
        }
        
        private class SubscriberToken
        {
            public string Name { get; set; }
            public Channel<object> Channel { get; set; }
            public ChannelWriter<object> Writer => Channel.Writer;
            
            public SubscriberToken(string name, Channel<object> channel)
            {
                Name    = name;
                Channel = channel;
            }
        }
    }
}
