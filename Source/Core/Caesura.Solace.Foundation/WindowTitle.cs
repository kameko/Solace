
namespace Caesura.Solace.Foundation
{
    using System;
    using System.Runtime.InteropServices;
    
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
            wt.ChangeTitle(title);
            return wt;
        }
        
        public void Dispose()
        {
            ChangeTitle(old_title);
        }
        
        private void ChangeTitle(string title)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.Title = title;
            }
        }
    }
}
