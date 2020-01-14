
namespace Solace.Core.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading.Channels;
    
    public abstract class MultiplexedSubsystem : BaseSubsystem
    {
        private Channel<object> MultiplexedChannel { get; set; }
        private List<ReaderSession> Sessions { get; set; }
        private readonly object SessionLock = new object();
        private volatile bool HaltExecution;
        
        public MultiplexedSubsystem() : base()
        {
            Sessions           = new List<ReaderSession>();
            MultiplexedChannel = Channel.CreateUnbounded<object>();
            
            HaltExecution = false;
        }
        
        protected abstract Task MultiplexedExecute(object message);
        
        protected override Task Execute()
        {
            if (HaltExecution)
            {
                return Task.CompletedTask;
            }
            
            var task = Task.Run(() =>
            {
                List<ReaderSession> sessions = null!;
                lock (SessionLock)
                {
                    sessions = new List<ReaderSession>(Sessions);
                }
                
                foreach (var session in sessions)
                {
                    while (session.Reader.TryRead(out var message))
                    {
                        MultiplexedChannel.Writer.TryWrite(message);
                    }
                }
            });
            
            task = task.ContinueWith(async task =>
            {
                while (MultiplexedChannel.Reader.TryRead(out var message))
                {
                    await MultiplexedExecute(message);
                }
            });
            
            task = task.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Log.Error(task.Exception!, string.Empty);
                    HaltExecution = true;
                }
            });
            
            return task;
        }
        
        public override void PlugInput(ChannelReader<object> reader)
        {
            var session = new ReaderSession(reader);
            session.Reader.Completion.ContinueWith(task =>
            {
                lock (SessionLock)
                {
                    Sessions.Remove(session);
                }
            });
            
            lock (SessionLock)
            {
                Sessions.Add(session);
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            MultiplexedChannel.Writer.TryComplete();
            HaltExecution = true;
        }
        
        private class ReaderSession
        {
            public Guid Id { get; private set; }
            public ChannelReader<object> Reader { get; set; }
            
            public ReaderSession(ChannelReader<object> reader)
            {
                Id     = Guid.NewGuid();
                Reader = reader;
            }
            
            public override string ToString()
            {
                return Id.ToString().ToUpper();
            }
        }
    }
}
