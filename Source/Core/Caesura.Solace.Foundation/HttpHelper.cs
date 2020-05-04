
namespace Caesura.Solace.Foundation
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Sockets;
    using Microsoft.AspNetCore.Http;
    
    public static class HttpHelper
    {
        public static string? RequestBodyAsString(HttpRequest request)
        {
            StreamReader? reader = null;
            try
            {
                reader = new StreamReader(request.Body);
                var response = reader.ReadToEnd();
                return response ?? string.Empty;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                reader?.Dispose();
            }
        }
        
        public static bool IsPortOpen(HttpClient client)
        {
            // I hate how I have to do any of this just to see
            // if a port is being used, but, that's the wonderful
            // world of information technology for you.
            
            var baseaddr = client.BaseAddress;
            var host     = baseaddr!.Host;
            var port     = baseaddr!.Port;
            
            TcpClient? tcp = null;
            try
            {
                tcp = new TcpClient();
                var async_result = tcp.BeginConnect(host, port, null, null);
                var success = async_result.AsyncWaitHandle.WaitOne(1_000);
                if (success)
                {
                    tcp.EndConnect(async_result);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                tcp?.Dispose();
            }
        }
    }
}
