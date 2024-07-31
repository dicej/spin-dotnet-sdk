using SpinHttpWorld;
using SpinHttpWorld.wit.imports.wasi.http.v0_2_0;
using SpinHttpWorld.wit.imports.wasi.io.v0_2_0;
using System.Text;

namespace SpinHttpWorld.wit.exports.wasi.http.v0_2_0;

public class IncomingHandlerImpl : IIncomingHandler
{
    private static Func<ITypes.IncomingRequest, ITypes.ResponseOutparam, Task>? _asyncHandler;
    public static Func<ITypes.IncomingRequest, ITypes.ResponseOutparam, Task>? AsyncHandler
    {
        set
        {
            if (value is not null && _simpleHandler is not null)
            {
                throw new Exception("only one of `AsyncHandler` or `SimpleHandler` may be set");
            }
            _asyncHandler = value;
        }
    }

    private static Func<Request, Response>? _simpleHandler;
    public static Func<Request, Response>? SimpleHandler
    {
        set
        {
            if (value is not null && _asyncHandler is not null)
            {
                throw new Exception("only one of `AsyncHandler` or `SimpleHandler` may be set");
            }
            _simpleHandler = value;
        }
    }

    /// <summary>Handle the specified incoming HTTP request and send a response
    /// via `responseOut`.</summary>
    public static void Handle(ITypes.IncomingRequest request, ITypes.ResponseOutparam responseOut)
    {
        var task = HandleAsync(request, responseOut);
        while (!task.IsCompleted)
        {
            WasiEventLoop.Dispatch();
        }
        var exception = task.Exception;
        if (exception is not null)
        {
            throw exception;
        }
    }

    /// <summary>Handle the specified incoming HTTP request and send a response
    /// via `responseOut`.</summary>
    static async Task HandleAsync(
        ITypes.IncomingRequest request,
        ITypes.ResponseOutparam responseOut
    )
    {
        if (_asyncHandler is not null)
        {
            await _asyncHandler(request, responseOut);
        }
        else if (_simpleHandler is not null)
        {
            var requestMethod = MethodString(request.Method());
            var requestBody = request.Consume();
            var requestBodyList = new List<byte>();
            try
            {
                using (var stream = new InputStream(requestBody.Stream()))
                {
                    var buffer = new byte[16 * 1024];
                    while (true)
                    {
                        var count = await stream.ReadAsync(buffer);
                        if (count == 0)
                        {
                            break;
                        }
                        else
                        {
                            requestBodyList.AddRange(buffer.Take(count));
                        }
                    }
                }
            }
            finally
            {
                ITypes.IncomingBody.Finish(requestBody);
            }

            var requestUri = request.PathWithQuery();
            if (requestUri is null)
            {
                requestUri = "/";
            }

            var requestHeaders = new Dictionary<string, byte[]>();
            foreach ((var key, var value) in request.Headers().Entries())
            {
                requestHeaders.Add(key, value);
            }

            Response simpleResponse;
            try
            {
                simpleResponse = _simpleHandler(
                    new Request(
                        requestMethod,
                        requestUri,
                        requestHeaders,
                        requestBodyList.ToArray()
                    )
                );
            }
            catch (Exception e)
            {
                var response = new ITypes.OutgoingResponse(ITypes.Fields.FromList(new()));
                response.SetStatusCode(500);
                var responseBody = response.Body();
                ITypes.ResponseOutparam.Set(
                    responseOut,
                    Result<ITypes.OutgoingResponse, ITypes.ErrorCode>.ok(response)
                );
                using (var sink = new OutputStream(responseBody.Write()))
                {
                    await sink.WriteAsync(Encoding.UTF8.GetBytes(e.ToString()));
                }
                ITypes.OutgoingBody.Finish(responseBody, null);
                return;
            }

            var responseHeaders = new List<(string, byte[])>();
            foreach (var pair in simpleResponse.headers)
            {
                responseHeaders.Add((pair.Key, pair.Value));
            }
            {
                var response = new ITypes.OutgoingResponse(ITypes.Fields.FromList(responseHeaders));
                response.SetStatusCode(simpleResponse.status);
                var responseBody = response.Body();
                ITypes.ResponseOutparam.Set(
                    responseOut,
                    Result<ITypes.OutgoingResponse, ITypes.ErrorCode>.ok(response)
                );
                if (simpleResponse.body is not null)
                {
                    using (var sink = new OutputStream(responseBody.Write()))
                    {
                        await sink.WriteAsync(simpleResponse.body);
                    }
                }
                ITypes.OutgoingBody.Finish(responseBody, null);
            }
        }
        else
        {
            var response = new ITypes.OutgoingResponse(ITypes.Fields.FromList(new()));
            response.SetStatusCode(500);
            var responseBody = response.Body();
            ITypes.ResponseOutparam.Set(
                responseOut,
                Result<ITypes.OutgoingResponse, ITypes.ErrorCode>.ok(response)
            );
            using (var sink = new OutputStream(responseBody.Write()))
            {
                await sink.WriteAsync(Encoding.UTF8.GetBytes("no request handler registered"));
            }
            ITypes.OutgoingBody.Finish(responseBody, null);
        }
    }

    static string MethodString(ITypes.Method method)
    {
        switch (method.Tag)
        {
            case ITypes.Method.GET:
                return "GET";
            case ITypes.Method.HEAD:
                return "HEAD";
            case ITypes.Method.POST:
                return "POST";
            case ITypes.Method.PUT:
                return "PUT";
            case ITypes.Method.DELETE:
                return "DELETE";
            case ITypes.Method.CONNECT:
                return "CONNECT";
            case ITypes.Method.OPTIONS:
                return "OPTIONS";
            case ITypes.Method.TRACE:
                return "TRACE";
            case ITypes.Method.PATCH:
                return "PATCH";
            case ITypes.Method.OTHER:
                return method.AsOther;
            default:
                throw new Exception("unreachable code");
        }
    }

    public readonly struct Request
    {
        public readonly string method;
        public readonly string uri;
        public readonly Dictionary<string, byte[]> headers;
        public readonly byte[]? body;

        public Request(string method, string uri, Dictionary<string, byte[]> headers, byte[]? body)
        {
            this.method = method;
            this.uri = uri;
            this.headers = headers;
            this.body = body;
        }
    }

    public readonly struct Response
    {
        public readonly ushort status;
        public readonly Dictionary<string, byte[]> headers;
        public readonly byte[]? body;

        public Response(ushort status, Dictionary<string, byte[]> headers, byte[]? body)
        {
            this.status = status;
            this.headers = headers;
            this.body = body;
        }
    }
}
