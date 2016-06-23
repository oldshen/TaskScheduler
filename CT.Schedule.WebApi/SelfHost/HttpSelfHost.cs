using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CT.Schedule.WebApi.SelfHost
{
    /// <summary>
    /// 自启动web服务器
    /// </summary>
    public class HttpSelfHost
    {
        public static readonly HttpSelfHost Instance = new HttpSelfHost();
        private HttpSelfHost() { }
        public void Start()
        {
            try
            {
                string hostPort = System.Configuration.ConfigurationManager.AppSettings["CTSchedule.WebHostPort"] ?? "8080";
                Uri[] uris = GetUriParams(Convert.ToInt32(hostPort));
                using (var host = new NancyHost(new CustomBootstrapper(), uris))
                {
                    host.Start();
                    Console.WriteLine("Running Nancy Host ,port:" + hostPort);
                    foreach(var item in uris) {
                        Console.WriteLine(item);
                    }
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private Uri[] GetUriParams(int port)
        {
            var uriParams = new List<Uri>();
            string hostName = Dns.GetHostName();

            // Host name URI
            string hostNameUri = string.Format("http://{0}:{1}", Dns.GetHostName(), port);
            uriParams.Add(new Uri(hostNameUri));

            // Host address URI(s)
            var hostEntry = Dns.GetHostEntry(hostName);
            foreach (var ipAddress in hostEntry.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)  // IPv4 addresses only
                {
                    var addrBytes = ipAddress.GetAddressBytes();
                    string hostAddressUri = string.Format("http://{0}.{1}.{2}.{3}:{4}",
                    addrBytes[0], addrBytes[1], addrBytes[2], addrBytes[3], port);
                    uriParams.Add(new Uri(hostAddressUri));
                }
            }
            // Localhost URI
            uriParams.Add(new Uri(string.Format("http://localhost:{0}", port)));
            return uriParams.ToArray();
        }
    }
}