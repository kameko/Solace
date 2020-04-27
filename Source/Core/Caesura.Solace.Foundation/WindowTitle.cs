
namespace Caesura.Solace.Foundation
{
    using System;
    
    public class WindowTitle : IDisposable
    {
        private string old_title;
        
        private WindowTitle(string old)
        {
            old_title = old;
        }
        
        public static WindowTitle Set(string title)
        {
            var wt = new WindowTitle(Console.Title);
            Console.Title = title;
            return wt;
        }
        
        public void Dispose()
        {
            Console.Title = old_title;
        }
    }
}
