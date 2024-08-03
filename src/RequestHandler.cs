using System.Text;
using SpinHttpWorld;
using SpinHttpWorld.wit.imports.wasi.http.v0_2_0;
using SpinHttpWorld.wit.imports.wasi.io.v0_2_0;

namespace Spin.Http;

public class RequestHandler
{
    public static void Run(Task task)
    {
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

        public static async Task<Request> FromAsync(ITypes.IncomingRequest request)
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

            return new Request(
                requestMethod,
                requestUri,
                requestHeaders,
                requestBodyList.ToArray()
            );
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

        public async Task SetAsync(ITypes.ResponseOutparam responseOut)
        {
            var responseHeaders = new List<(string, byte[])>();
            foreach (var pair in this.headers)
            {
                responseHeaders.Add((pair.Key, pair.Value));
            }

            var response = new ITypes.OutgoingResponse(ITypes.Fields.FromList(responseHeaders));
            response.SetStatusCode(this.status);
            var responseBody = response.Body();
            ITypes.ResponseOutparam.Set(
                responseOut,
                Result<ITypes.OutgoingResponse, ITypes.ErrorCode>.ok(response)
            );
            if (this.body is not null)
            {
                using (var sink = new OutputStream(responseBody.Write()))
                {
                    await sink.WriteAsync(this.body);
                }
            }
            ITypes.OutgoingBody.Finish(responseBody, null);
        }
    }
}
