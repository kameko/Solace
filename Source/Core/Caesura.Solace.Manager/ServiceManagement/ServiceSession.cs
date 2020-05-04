
namespace Caesura.Solace.Manager.ServiceManagement
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using Foundation.ConfigurationModels;
    using Foundation.ApiBoundaries.HttpClients;
    
    public class ServiceSession : IDisposable
    {
        public string Name { get; private set; }
        public FileInfo ExecutablePath { get; private set; }
        public Uri ConnectionPath { get; private set; }
        public bool Local { get; set; }
        public bool Autostart { get; set; }
        public bool Autoclose { get; set; }
        public bool CreateWindow { get; set; }
        
        public IBaseClient Client { get; set; }
        public Process? Handle { get; set; }
        
        public ServiceSession(string name, IBaseClient client, ServicesModel.Service model)
            : this(
                name,
                client,
                model.ExecPath,
                model.Connection,
                model.Local,
                model.Autostart,
                model.Autoclose,
                model.CreateWindow
            )
        {
            
        }
        
        public ServiceSession(
            string name,
            IBaseClient client,
            string exec_path,
            string connection_path,
            bool local,
            bool autostart,
            bool autoclose,
            bool create_window
        )
            : this(
                name,
                client,
                new FileInfo(exec_path),
                new Uri(connection_path, UriKind.Absolute),
                local,
                autostart,
                autoclose,
                create_window
            )
        {
            
        }
        
        public ServiceSession(
            string name,
            IBaseClient client,
            FileInfo exec_info,
            Uri connection_uri,
            bool local,
            bool autostart,
            bool autoclose,
            bool create_window
        )
        {
            Name           = name;
            Client         = client;
            ExecutablePath = exec_info;
            ConnectionPath = connection_uri;
            Local          = local;
            Autostart      = autostart;
            Autoclose      = autoclose;
            CreateWindow   = create_window;
        }
        
        public static ValidationResult TryCreate(string name, IBaseClient client, ServicesModel.Service model, out ServiceSession? session)
        {
            var uri_success = Uri.TryCreate(model.Connection, UriKind.Absolute, out var uri);
            if (uri_success)
            {
                var exec_fi = new FileInfo(model.ExecPath);
                session = new ServiceSession(
                    name,
                    client,
                    exec_fi,
                    uri!,
                    model.Local,
                    model.Autostart,
                    model.Autoclose,
                    model.CreateWindow
                );
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
        
        public void Dispose()
        {
            Handle?.Dispose();
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
