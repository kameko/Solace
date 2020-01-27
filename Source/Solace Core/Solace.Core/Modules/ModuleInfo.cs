
namespace Solace.Core.Modules
{
    using System;
    using System.Text.Json;
    
    public class ModuleInfo
    {
        // Comparable data, what we care about when comparing modules.
        public string Name { get; set; }
        public string Company { get; set; }
        public Version Version { get; set; }
        
        // Non-comparable data.
        public string Copyright { get; set; }
        
        public ModuleInfo()
        {
            Name         = string.Empty;
            Company      = string.Empty;
            Version      = new Version();
            Copyright    = string.Empty;
        }
        
        public static bool operator == (ModuleInfo mi1, ModuleInfo mi2)
        {
            if (mi1 is null && mi2 is null)
            {
                return true;
            }
            if (mi1 is null || mi2 is null)
            {
                return false;
            }
            return (
                   mi1.Name.Equals(mi2.Name)
                && mi1.Company.Equals(mi2.Company)
                && mi1.Version == mi2.Version
            );
        }
        
        public static bool operator != (ModuleInfo mi1, ModuleInfo mi2)
        {
            return !(mi1 == mi2);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is ModuleInfo mi)
            {
                return this == mi;
            }
            return false;
        }
        
        public override int GetHashCode()
        {
            return new
            {
                Name,
                Company,
                Version
            }
            .GetHashCode();
        }
        
        public override string ToString()
        {
            var opt = new JsonSerializerOptions();
            return ToString(opt);
        }
        
        public string ToString(JsonSerializerOptions opt)
        {
            var json = JsonSerializer.Serialize<ModuleInfo>(this, opt);
            return json;
        }
    }
}
