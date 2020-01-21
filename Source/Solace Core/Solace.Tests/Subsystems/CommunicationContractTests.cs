
namespace Solace.Tests.Subsystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Subsystems;
    using Xunit;
    
    public class CommunicationContractTests
    {
        [Fact]
        public void Test1()
        {
            var cc = CommunicationContract.Create("N1", "N2");
            
            var subscriber = cc.Subscriber;
            var producer = cc.Producer;
            
            var success1 = subscriber.Send(new Message("Hello"));
            
            Assert.True(success1);
            
            var success2 = producer.Receive(out var message1);
            
            Assert.True(success2);
            Assert.Equal("Hello", message1.Command);
            
            var success3 = producer.Send(new Message("Hi"));
            
            Assert.True(success3);
            
            var success4 = subscriber.Receive(out var message2);
            
            Assert.True(success4);
            Assert.Equal("Hi", message2.Command);
        }
    }
}
