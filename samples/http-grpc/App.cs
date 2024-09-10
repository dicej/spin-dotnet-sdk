using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Spin.Http;
using SpinHttpWorld.wit.imports.wasi.http.v0_2_1;

namespace SpinHttpWorld.wit.exports.wasi.http.v0_2_1;

public class IncomingHandlerImpl : IIncomingHandler
{
    /// <summary>Handle the specified incoming HTTP request and send a response
    /// via `responseOut`.</summary>
    public static void Handle(ITypes.IncomingRequest request, ITypes.ResponseOutparam responseOut)
    {
        RequestHandler.Run(HandleAsync(request, responseOut));
    }

    static async Task HandleAsync(ITypes.IncomingRequest request, ITypes.ResponseOutparam responseOut)
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7042", new GrpcChannelOptions
        {
            // TODO: remove once System.Net.Sockets support is available
            HttpHandler = new HttpClientHandler()
        });
        var client = new Greeter.GreeterClient(channel);
        var reply = await client.SayHelloAsync(new HelloRequest { Name = "gRPC" });
        var greeting = $"Hello, {reply.Message}!";
        RequestHandler.Run(
            new RequestHandler.Response(
                200,
                new Dictionary<string, byte[]>
                {
                    { "content-type", Encoding.UTF8.GetBytes("text/plain") }
                },
                Encoding.UTF8.GetBytes(greeting)
            ).SetAsync(responseOut)
        );
    }
}
