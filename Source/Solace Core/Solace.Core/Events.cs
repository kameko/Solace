
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Modules;
    
    public class Events
    {
        public bool SanitizeEventNames { get; private set; }
        private List<EventToken> Tokens { get; set; }
        private List<BlockSession> BlockSessions { get; set; }
        
        public Events(bool sanitize_names)
        {
            SanitizeEventNames = sanitize_names;
            Tokens             = new List<EventToken>();
            BlockSessions      = new List<BlockSession>();
        }
        
        public Events() : this(true)
        {
            
        }
        
        public EventRegistration Register(ModuleInfo info)
        {
            var er = new EventRegistration(this, info);
            return er;
        }
        
        public void DestroyEvent(string event_name)
        {
            var name = SanitizeEventNames ? SanitizeName(event_name) : event_name;
            Tokens.RemoveAll(
                x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
            );
        }
        
        public static string SanitizeName(string name)
        {
            var nn = name;
            nn = nn.Replace(' ', '_');
            nn = new string(nn.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-').ToArray());
            nn = nn.ToUpper();
            return nn;
        }
        
        internal bool TokenExists(EventToken token)
        {
            var name = SanitizeEventNames ? SanitizeName(token.EventName) : token.EventName;
            return Tokens.Exists(
                x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                  && x.Callback == token.Callback
            );
        }
        
        internal IEnumerable<EventToken> GetTokens(string event_name)
        {
            var name = SanitizeEventNames ? SanitizeName(event_name) : event_name;
            return Tokens.Where(x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
        
        internal void AddToken(EventToken token)
        {
            if (SanitizeEventNames)
            {
                token.EventName = SanitizeName(token.EventName);
            }
            if (!TokenExists(token))
            {
                Tokens.Add(token);
            }
        }
        
        internal void RemoveToken(string event_name, ModuleInfo info, EventCallback callback)
        {
            var name = SanitizeEventNames ? SanitizeName(event_name) : event_name;
            var token = Tokens.Find(
                x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                  && x.Callback == callback
            );
            if (!(token is null))
            {
                Tokens.Remove(token);
            }
        }
        
        internal void RemoveAllTokens(string event_name, ModuleInfo info)
        {
            var name = SanitizeEventNames ? SanitizeName(event_name) : event_name;
            Tokens.RemoveAll(
                x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                  && x.Module == info
            );
        }
        
        internal void Raise(string name, ModuleInfo info)
        {
            throw new NotImplementedException();
        }
        
        internal void Raise(string name, ModuleInfo info, object data)
        {
            throw new NotImplementedException();
        }
        
        internal void Block(string name, ModuleInfo info)
        {
            throw new NotImplementedException();
        }
        
        internal void Release(string name, ModuleInfo info)
        {
            throw new NotImplementedException();
        }
        
        private class BlockSession
        {
            public string EventName { get; set; }
            public ModuleInfo BlockingModule { get; set; }
            
            public BlockSession(string name, ModuleInfo module)
            {
                EventName      = name;
                BlockingModule = module;
            }
        }
    }
}
