
namespace Solace.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
    public static class ObjectExtensions
    {
        public static string GetObjectDifferencesAsString(this object before, object after)
        {
            var diff = string.Empty;
            var before_props = before.GetType().GetProperties();
            var after_props = before.GetType().GetProperties();
            for (var i = 0; i < before_props.Count(); i++)
            {
                // TODO: handle indexable properties.
                // I'm thinking it should be a bool to handle them
                // or just an entire different method (or both),
                // because some methods just might not care.
                
                var before_prop = before_props.ElementAt(i);
                var before_prop_value = before_prop.GetValue(before, before_prop.GetIndexParameters());
                var after_prop = after_props.ElementAt(i);
                var after_prop_value = after_prop.GetValue(after, after_prop.GetIndexParameters());
                if (!before_prop_value!.Equals(after_prop_value))
                {
                    var prop_name = before_prop.Name;
                    if (!string.IsNullOrEmpty(diff))
                    {
                        diff += ", ";
                    }
                    diff += $"{prop_name}: {before_prop_value} -> {after_prop_value}";
                }
            }
            return diff;
        }
    }
}
