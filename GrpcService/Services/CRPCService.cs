using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Google.Protobuf.WellKnownTypes;

namespace GrpcService
{
    public class CRPCService : Greeter.GreeterBase
    {
        private readonly ILogger<CRPCService> _logger;
        public CRPCService(ILogger<CRPCService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<MessageTextList> GetMessageList(MessageTextRequest request, ServerCallContext context)
        {
            return Task.FromResult(new MessageTextList
            {
                MessageList = { new MessageText { Id = 0, Text = "TestMessage", Date = Timestamp.FromDateTime(DateTime.Now), Type = 0 } }
            });
        }

        const string _FileAppDirectory = @"C:\inetpub\wwwroot\P3teProject\";

        public override Task<MessageAppFileList> GetAppFileList(Empty _null, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                MessageAppFileList _list = new MessageAppFileList();

                foreach (var _File in Directory.GetFiles(_FileAppDirectory))
                {
                    if (!_File.Contains("web.config"))
                    {
                        FileInfo _FileInfo = new FileInfo(_File);
                        _list.FileList.Add(new MessageAppFile { FilePath = Path.GetRelativePath(_FileAppDirectory, _File), WriteTime = Timestamp.FromDateTime(_FileInfo.LastWriteTimeUtc) });
                    }
                }

                foreach (var _Dir in Directory.GetDirectories(_FileAppDirectory))
                {
                    foreach (var _File in Directory.GetFiles(_Dir))
                    {

                        FileInfo _FileInfo = new FileInfo(_File);
                        _list.FileList.Add(new MessageAppFile { FilePath = Path.GetRelativePath(_FileAppDirectory, _File), WriteTime = Timestamp.FromDateTime(_FileInfo.LastWriteTimeUtc) });
                    }
                }

                return _list;
            });
        }

        public override async Task Subscribe(MessageSubscribe _MessageSubscribe
            , IServerStreamWriter<MessageSubscribeResponse> _ResponseStream
            , ServerCallContext context)
        {
            await Task.Run(async () =>
            {
                for (int i = 0; i < 5; ++i)
                {
                    await _ResponseStream.WriteAsync(new MessageSubscribeResponse { MethodName = "CallThisMethod()", Parameters = "Parameter0" });

                    await Task.Delay(10000);

                    await _ResponseStream.WriteAsync(new MessageSubscribeResponse { MethodName = "ShowMessage", Parameters = "Man Server Hastam! :D" });
                }
            });
        }

        public override Task<MessageLastLogID> GetLastLogId(Empty _null, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                using (var db = new Database.AppDataBase())
                {
                    return new MessageLastLogID { LogErrorId = db.LogErrors.Last().Id, LogInfoId = db.LogInfo.Last().Id };
                }
            });
        }

        public override Task<Empty> SendLogs(MessageLogs request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                using (var db = new Database.AppDataBase())
                {
                    foreach (var _LogError in request.LogErrorList.ToList())
                    {
                        db.LogErrors.Add(new Database.DBLogError { Context = _LogError.Text, Date = _LogError.Date.ToDateTime() });
                    }

                    foreach (var _LogInfo in request.LogInfoList.ToList())
                    {
                        db.LogInfo.Add(new Database.DBLogInfo { Context = _LogInfo.Text, Date = _LogInfo.Date.ToDateTime() });
                    }
                }

                return new Empty();
            });
        }

    }
}
