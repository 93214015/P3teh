using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using System.Net.Http; // HttpClientHandler

using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Grpc.Net.Client.Web;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;
using Grpc.Core;

namespace WPF.NETCore.gRPC
{
    class RPCClient
    {

        const string m_ServerIPAddress = "https://87.236.212.221:5001";
        //const string m_ServerIPAddress = "http://127.0.0.1:5001";
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

        public static async Task SyncLogs()
        {
            try
            {

                var client = new Greeter.GreeterClient(m_Channel);

                var respose = await client.GetLastLogIdAsync(new Empty());

                MessageLogs _MessageLogs = new MessageLogs();

                using (var db = new Database.AppDataBase())
                {
                    foreach (var _Error in db.LogErrors.Where(error => error.Id > respose.LogErrorId).ToList())
                    {
                        _MessageLogs.LogErrorList.Add(new MessageLog { Text = _Error.Context, Date = Timestamp.FromDateTime(_Error.Date.ToUniversalTime()) });
                    }

                    foreach (var _Info in db.LogInfo.Where(info => info.Id > respose.LogInfoId).ToList())
                    {
                        _MessageLogs.LogInfoList.Add(new MessageLog { Text = _Info.Context, Date = Timestamp.FromDateTime(_Info.Date.ToUniversalTime()) });
                    }
                }

                if (_MessageLogs.LogErrorList.Count > 0 || _MessageLogs.LogInfoList.Count > 0)
                {
                    await client.SendLogsAsync(_MessageLogs);
                }
            }
            catch (Exception ex)
            {
                MainWindow.ShowMessage(ex.Message);
                MainWindow.ShowMessage("خطایی در ارتباط با سرور پیش آمده است");
            }
        }

        public static void Subscribe()
        {

            try
            {
                var client = new Greeter.GreeterClient(m_Channel);

                var _SubscribedStream = client.Subscribe(new MessageSubscribe { Id = 0 });

                Task.Run(async () =>
                {
                    try
                    {
                        await foreach (var update in _SubscribedStream.ResponseStream.ReadAllAsync())
                        {
                            switch (update.MethodName)
                            {
                                case "ShowMessage":
                                    MainWindow.ShowMessage(update.Parameters);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch { }
                });

            }
            catch
            {
                MainWindow.ShowMessage("خطایی در ارتباط با سرور پیش آمده است");
            }
        }

    }
}
