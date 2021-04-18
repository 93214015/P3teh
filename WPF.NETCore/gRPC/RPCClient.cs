using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Http; // HttpClientHandler

using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;


namespace WPF.NETCore.gRPC
{
    class RPCClient
    {

        const string m_ServerIPAddress = "https://87.236.212.221:5001";
        static GrpcChannel m_Channel;

        public static void Init()
        {

            var defaultMethodConfigs = new MethodConfig
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 5,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1.5,
                    RetryableStatusCodes = { Grpc.Core.StatusCode.Unavailable }
                }
            };

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            m_Channel = GrpcChannel.ForAddress(m_ServerIPAddress, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(httpClientHandler)
            });

        }

        public static async Task<MessageAppFileList> GetUpdateFiles()
        {
            var client = new Greeter.GreeterClient(m_Channel);
            var _FileList = await client.GetAppFileListAsync(new Empty());
            return _FileList;
        }

    }
}
