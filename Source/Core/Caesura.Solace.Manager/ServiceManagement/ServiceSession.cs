
namespace Caesura.Solace.Manager.ServiceManagement
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using ConfigurationModels;
    
    public class ServiceSession
    {
        public string Name { get; private set; }
        public FileInfo ExecutablePath { get; private set; }
        public Uri ConnectionPath { get; private set; }
        public bool Local { get; set; }
        public Process? Handle { get; set; }
        
        public ServiceSession(string name, ServicesModel.Service model)
            : this(name, model.ExecPath, model.Connection, model.Local)
        {
            
        }
        
        public ServiceSession(string name, string exec_path, string connection_path, bool local)
            : this(name, new FileInfo(exec_path), new Uri(connection_path, UriKind.Absolute), local)
        {
            
        }
        
        public ServiceSession(string name, FileInfo exec_info, Uri connection_uri, bool local)
        {
            Name           = name;
            ExecutablePath = exec_info;
            ConnectionPath = connection_uri;
            Local          = local;
        }
        
        public static ValidationResult TryCreate(string name, ServicesModel.Service model, out ServiceSession? session)
        {
            var uri_success = Uri.TryCreate(model.Connection, UriKind.Absolute, out var uri);
            if (uri_success)
            {
                var exec_fi = new FileInfo(model.ExecPath);
                session = new ServiceSession(name, exec_fi, uri!, model.Local);
                var exec_verification = File.Exists(exec_fi.FullName);
                if (exec_verification)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return ValidationResult.InvalidPath;
                }
            }
            else
            {
                session = null;
                return ValidationResult.InvalidUri;
            }
        }
        
        public static bool IsValid(ValidationResult result)
        {
            return result == ValidationResult.Success;
        }
        
        public enum ValidationResult
        {
            None,
            Success,
            InvalidUri,
            InvalidPath,
        }
    }
}
