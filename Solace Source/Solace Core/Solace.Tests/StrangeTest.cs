
namespace Solace.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text.Json;
    using Xunit;
    using Xunit.Abstractions;
    
    public class StrangeTest : BaseTest
    {
        public StrangeTest(ITestOutputHelper output) : base(output)
        {
            
        }
        
        [Fact]
        public void Test1()
        {
            var dt = new DateTime(2020, 1, 1);
            Write(dt.ToString());
            var json = JsonSerializer.Serialize(dt);
            Write(json);
            var dt2 = JsonSerializer.Deserialize<DateTime>(json);
            Write(dt2.ToString());
        }
    }
}
