
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public static class ObjectExtensions
    {
        public static string GetShallowObjectDifferencesAsString(this object before, object after)
        {
            var diff = string.Empty;
            var before_props = before.GetType().GetProperties();
            var after_props = before.GetType().GetProperties();
            for (var i = 0; i < before_props.Count(); i++)
            {
                var before_prop       = before_props.ElementAt(i);
                var before_prop_value = before_prop.GetValue(before, before_prop.GetIndexParameters());
                var after_prop        = after_props.ElementAt(i);
                var after_prop_value  = after_prop.GetValue(after, after_prop.GetIndexParameters());
                
                if (before_prop.GetIndexParameters().Count() > 0)
                {
                    continue;
                }
                
                if (!before_prop_value!.Equals(after_prop_value))
                {
                    diff += $"{before_prop.Name}: {before_prop_value} -> {after_prop_value}, ";
                }
            }
            
            return diff.Substring(0, diff.Length - 2);
        }
        
        public static string GetDeepObjectDifferencesAsString(this object before, object after)
        {
            // TODO: implement deep object difference checking
            throw new NotImplementedException();
        }
    }
}
