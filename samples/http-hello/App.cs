using Spin.Http;
using SpinHttpWorld.wit.imports.wasi.http.v0_2_0;
using System.Text;

namespace SpinHttpWorld.wit.exports.wasi.http.v0_2_0;

public class IncomingHandlerImpl : IIncomingHandler
{
    /// <summary>Handle the specified incoming HTTP request and send a response
    /// via `responseOut`.</summary>
    public static void Handle(ITypes.IncomingRequest request, ITypes.ResponseOutparam responseOut)
    {
        RequestHandler.Run(
            new RequestHandler.Response(
                200,
                new Dictionary<string, byte[]>
                {
                    { "content-type", Encoding.UTF8.GetBytes("text/plain") }
                },
                Encoding.UTF8.GetBytes("hello, world!")
            ).SetAsync(responseOut)
        );
    }
}
