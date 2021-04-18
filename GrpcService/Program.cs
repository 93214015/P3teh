using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GrpcService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(kestrel =>
                    {

                        //kestrel.Listen(IPAddress.Any, 5001 /*, listenOptions =>
                        //{
                        //    listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                        //    listenOptions.UseHttps(@"Certificate\certificate.p12", "taghipour93214015");
                        //}*/);

                        kestrel.ConfigureHttpsDefaults(https =>
                        {
                            https.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(@"Certificate\certificate.p12", "taghipour93214015");
                        });
                    });
                });
    }
}
