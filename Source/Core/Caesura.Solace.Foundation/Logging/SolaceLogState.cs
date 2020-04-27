
namespace Caesura.Solace.Foundation.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text.Json;
    
    public class SolaceLogState
    {
        public string Message { get; set; }
        public List<StateElement> Values { get; set; }
        
        public SolaceLogState(string message, List<StateElement> values)
        {
            Message = message;
            Values  = values;
        }
        
        public static SolaceLogState Create(string message, object[] args)
        {
            var elms  = new List<StateElement>(args.Length);
            var count = 0;
            foreach (var item in args)
            {
                var elm = new StateElement(count, item);
                elms.Add(elm);
                count++;
            }
            var state = new SolaceLogState(message, elms);
            return state;
        }
        
        public string ToJson(bool indent)
        {
            var opt = new JsonSerializerOptions()
            {
                WriteIndented = indent,
            };
            
            // TODO: string.Split() { and } and replace everything between
            // them with JSON depending on index. Save the position of each
            // label so a message can do Log("{Id0} {Id0} {Id1}", id0, id1).
            // TODO: serialize all non-primitive types. ints, strings, etc..
            // should be printed as-is.
            
            return Message;
        }
        
        public override string ToString()
        {
            return ToJson(indent: true);
        }
        
        public class StateElement
        {
            public int Position { get; set; }
            public object Value { get; set; }
            
            public StateElement(int pos, object val)
            {
                Position = pos;
                Value    = val;
            }
        }
    }
}
