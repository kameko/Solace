
namespace Solace.Tests.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Subsystems;
    using Xunit;
    
    public class SubsystemTests
    {
        [Fact]
        public void Test1()
        {
            var sm = new SubsystemManager();
            var ping = new PingSubsystem();
            var pong = new PongSubsystem();
            sm.Add(ping);
            sm.Add(pong);
            
        }
        
        private class PingSubsystem : BaseSubsystem
        {
            private int PingCount { get; set; }
            
            public PingSubsystem() : base()
            {
                Name = nameof(PingSubsystem);
            }
            
            protected override Task Pulse(IEnumerable<Message> messages)
            {
                foreach (var message in messages)
                {
                    if (message.Command == "PING")
                    {
                        message.Respond("PONG");
                        
                        PingCount++;
                        if (PingCount > 10)
                        {
                            message.CloseChannel();
                        }
                    }
                }
                
                return Task.CompletedTask;
            }
        }
        
        private class PongSubsystem : BaseSubsystem
        {
            private int PongCount { get; set; }
            
            public PongSubsystem() : base()
            {
                Name = nameof(PongSubsystem);
            }
            
            protected override Task Pulse(IEnumerable<Message> messages)
            {
                foreach (var message in messages)
                {
                    if (message.Command == "PONG")
                    {
                        message.Respond("PING");
                        
                        PongCount++;
                        if (PongCount > 10)
                        {
                            message.CloseChannel();
                        }
                    }
                }
                
                return Task.CompletedTask;
            }
        }
    }
}
