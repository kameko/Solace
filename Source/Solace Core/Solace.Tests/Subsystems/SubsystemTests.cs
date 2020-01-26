
namespace Solace.Tests.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Subsystems;
    using Xunit;
    using Xunit.Abstractions;
    
    public class SubsystemTests
    {
        private readonly ITestOutputHelper output;
        
        public SubsystemTests(ITestOutputHelper output)
        {
            this.output = output;
            WriteLine(string.Empty);
            
            Core.Log.OnLog += log =>
            {
                string file = log.CallerFilePath.Split('\\', '/').Last().Replace(".cs", "");
                WriteLine(
                    $"[{log.Level.ToString().ToUpper()} {file}@" +
                    $"{log.CallerMemberName}:{log.CallerLineNumber}] {log.Message}"
                );
            };
        }
        
        protected void WriteLine(string message)
        {
            output.WriteLine(message);
            Console.WriteLine(message);
        }
        
        [Fact]
        public void Test1()
        {
            var sm   = new SubsystemManager();
            var ping = new PingSubsystem(output);
            var pong = new PongSubsystem(output);
            sm.Add(ping);
            sm.Add(pong);
            
            var form_contract_success = sm.FormCommunicationContract(ping.Name, pong.Name, out var contract);
            Assert.True(form_contract_success);
            
            var send_success = contract!.AsProducer(new Message<int>("PING", 0));
            Assert.True(send_success);
            
            WriteLine(contract.ToString());
            
            var ct = new CancellationTokenSource();
            ct.CancelAfter(3000);
            sm.Run(ct.Token);
            
            WriteLine(contract.ToString());
        }
        
        private class PingSubsystem : BaseSubsystem
        {
            private readonly ITestOutputHelper output;
            private int PingCount { get; set; }
            
            public PingSubsystem(ITestOutputHelper output) : base()
            {
                this.output = output;
                Name = nameof(PingSubsystem);
            }
            
            protected override Task Pulse(Message message)
            {
                if (message is Message<int> mi && mi.Command == "PING")
                {
                    WriteLine($"PONG {mi} ({PingCount})");
                    mi.Respond(new Message<int>("PONG", mi.Data + 1));
                    
                    PingCount++;
                    if (PingCount >= 10)
                    {
                        mi.CloseChannel();
                    }
                }
                else
                {
                    WriteLine($"{Name} Got unexpected message: {message}");
                }
                
                return Task.CompletedTask;
            }
            
            protected void WriteLine(string message)
            {
                output.WriteLine(message);
                Console.WriteLine(message);
            }
        }
        
        private class PongSubsystem : BaseSubsystem
        {
            private readonly ITestOutputHelper output;
            private int PongCount { get; set; }
            
            public PongSubsystem(ITestOutputHelper output) : base()
            {
                this.output = output;
                Name = nameof(PongSubsystem);
            }
            
            protected override Task Pulse(Message message)
            {
                if (message is Message<int> mi && mi.Command == "PONG")
                {
                    WriteLine($"PING {mi} ({PongCount})");
                    mi.Respond(new Message<int>("PING", mi.Data + 1));
                    
                    PongCount++;
                    if (PongCount >= 10)
                    {
                        mi.CloseChannel();
                    }
                }
                else
                {
                    WriteLine($"{Name} Got unexpected message: {message}");
                }
                
                return Task.CompletedTask;
            }
            
            protected void WriteLine(string message)
            {
                output.WriteLine(message);
                Console.WriteLine(message);
            }
        }
    }
}
