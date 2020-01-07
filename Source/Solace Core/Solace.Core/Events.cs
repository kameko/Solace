
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Modules;
    
    public class Events
    {
        public bool SanitizeEventNames { get; private set; }
        private List<EventToken> Tokens { get; set; }
        private List<BlockSession> BlockSessions { get; set; }
        private readonly object BlockLock = new object();
        
        internal CancellationToken CancelToken { get; set; }
        
        public Events(bool sanitize_names)
        {
            SanitizeEventNames = sanitize_names;
            Tokens             = new List<EventToken>();
            BlockSessions      = new List<BlockSession>();
            CancelToken        = new CancellationToken();
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
        
        private string MaybeSanitize(string name)
        {
            return SanitizeEventNames ? SanitizeName(name) : name;
        }
        
        internal bool TokenExists(EventToken token)
        {
            var name = MaybeSanitize(token.EventName);
            return Tokens.Exists(
                x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                  && x.Callback == token.Callback
            );
        }
        
        internal IEnumerable<EventToken> GetTokens(string event_name)
        {
            var name = MaybeSanitize(event_name);
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
            var name = MaybeSanitize(event_name);
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
            var name = MaybeSanitize(event_name);
            Tokens.RemoveAll(
                x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                  && x.Module == info
            );
        }
        
        internal void Raise(string event_name, ModuleInfo info)
        {
            var msg = new ModuleMessage(info);
            Raise(event_name, msg);
        }
        
        internal void Raise(string event_name, ModuleInfo info, object data)
        {
            var msg = new ModuleMessage(info, data);
            Raise(event_name, msg);
        }
        
        internal void Raise(string event_name, ModuleMessage message)
        {
            var name = MaybeSanitize(event_name);
            Task.Run(() =>
                {
                    if (IsBlocked(event_name))
                    {
                        // check if the event is blocked, if so, acqure a lock
                        // and then check if it's blocked again. More than likely
                        // this double-check will be cheaper than always getting a lock.
                        lock (BlockLock)
                        {
                            if (IsBlocked(event_name))
                            {
                                var bs = BlockSessions.Find(
                                    x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                                );
                                if (!(bs is null) && bs.BlockingModule != message.Sender)
                                {
                                    // This event is blocked and the caller is not the blocker.
                                    return;
                                }
                            }
                        }
                    }
                    
                    foreach (var token in Tokens)
                    {
                        if (CancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        
                        token.Execute(CancelToken, message);
                    }
                },
                CancelToken
            ).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        // TODO: handle fault
                    }
                }
            );
        }
        
        internal void Block(string event_name, ModuleInfo info)
        {
            var name = MaybeSanitize(event_name);
            lock (BlockLock)
            {
                if (!IsBlocked(event_name))
                {
                    var bs = new BlockSession(name, info);
                    BlockSessions.Add(bs);
                }
            }
        }
        
        internal void Release(string event_name, ModuleInfo info)
        {
            var name = MaybeSanitize(event_name);
            lock (BlockLock)
            {
                var bs = BlockSessions.Find(
                    x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                    && x.BlockingModule == info
                );
                if (!(bs is null))
                {
                    BlockSessions.Remove(bs);
                }
            }
        }
        
        internal bool IsBlocked(string event_name)
        {
            var name = MaybeSanitize(event_name);
            return BlockSessions.Exists(x => x.EventName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
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
