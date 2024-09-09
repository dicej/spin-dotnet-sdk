using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using Spin.Http;
using SpinHttpWorld.wit.imports.wasi.http.v0_2_1;

namespace SpinHttpWorld.wit.exports.wasi.http.v0_2_1;

public class WasiHttpServer : IServer
{
    private static Func<ITypes.IncomingRequest, ITypes.ResponseOutparam, Task>? _handleRequest;

    public IFeatureCollection Features { get; } = new FeatureCollection();

    public WasiHttpServer(IHostApplicationLifetime lifetime) { }

    public Task StartAsync<TContext>(
        IHttpApplication<TContext> application,
        CancellationToken cancellationToken
    )
        where TContext : notnull
    {
        _handleRequest = async (request, responseOut) =>
        {
            var requestContext = new RequestContext(request, responseOut);

            var requestFeatures = new FeatureCollection();
            requestFeatures[typeof(IHttpRequestFeature)] = requestContext;
            requestFeatures[typeof(IHttpRequestBodyDetectionFeature)] = requestContext;
            requestFeatures[typeof(IHttpResponseFeature)] = requestContext;
            requestFeatures[typeof(IHttpResponseBodyFeature)] = requestContext;

            var ctx = application.CreateContext(requestFeatures);
            try
            {
                await application.ProcessRequestAsync(ctx);
                application.DisposeContext(ctx, null);
                await requestContext.CompleteAsync();
            }
            catch (Exception ex)
            {
                application.DisposeContext(ctx, ex);
                throw;
            }
        };

        return Task.CompletedTask;
    }

    public static async Task HandleRequestAsync(
        ITypes.IncomingRequest request,
        ITypes.ResponseOutparam responseOut
    )
    {
        if (_handleRequest is not null)
        {
            await _handleRequest(request, responseOut);
        }
        else
        {
            throw new Exception("WasiHttpServer not yet initialized");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void Dispose() { }

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

    class RequestContext
        : IHttpRequestFeature,
            IHttpResponseFeature,
            IHttpResponseBodyFeature,
            IHttpRequestBodyDetectionFeature
    {
        private List<(Func<object, Task>, object)> _onStartingCallbacks = new();
        private List<(Func<object, Task>, object)> _onCompletedCallbacks = new();
        private ITypes.IncomingBody _requestBody;
        private ITypes.ResponseOutparam _responseOut;
        private ITypes.OutgoingBody? _responseBody;
        private Stream? _stream;
        private PipeWriter? _writer;

        internal RequestContext(ITypes.IncomingRequest request, ITypes.ResponseOutparam responseOut)
        {
            var headers = new HeaderDictionary();
            foreach ((var key, var value) in request.Headers().Entries())
            {
                headers.Add(key, Encoding.UTF8.GetString(value));
            }

            var url = request.PathWithQuery();
            if (url is null)
            {
                url = "/";
            }

            var queryStartPos = url.IndexOf('?');
            Path = queryStartPos < 0 ? url : url.Substring(0, queryStartPos);
            QueryString = queryStartPos < 0 ? string.Empty : url.Substring(queryStartPos);
            Method = MethodString(request.Method());
            ((IHttpRequestFeature)this).Headers = headers;
            _requestBody = request.Consume();
            ((IHttpRequestFeature)this).Body = new InputStream(_requestBody.Stream());

            _responseOut = responseOut;
        }

        public string Protocol { get; set; } = "HTTP/1.1";
        public string Scheme { get; set; } = "http";
        public string Method { get; set; } = "GET";
        public string PathBase { get; set; } = string.Empty;
        public string Path { get; set; } = "/";
        public string QueryString { get; set; } = string.Empty;
        public string RawTarget
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public bool CanHaveBody { get; } = true;

        // TODO: Prohibit changing the status code or response headers if
        // `_stream is not null` (i.e. if we've already started sending the
        // response)
        public int StatusCode { get; set; } = 200;
        public string? ReasonPhrase { get; set; }

        public bool HasStarted { get; }

        public Stream Stream
        {
            get
            {
                if (_stream is null)
                {
                    var responseHeaders = new List<(string, byte[])>();
                    foreach (var pair in ((IHttpResponseFeature)this).Headers)
                    {
                        responseHeaders.Add(
                            (pair.Key, Encoding.UTF8.GetBytes(pair.Value.ToString()))
                        );
                    }

                    var response = new ITypes.OutgoingResponse(
                        ITypes.Fields.FromList(responseHeaders)
                    );
                    response.SetStatusCode((ushort)StatusCode);
                    _responseBody = response.Body();
                    ITypes.ResponseOutparam.Set(
                        _responseOut,
                        Result<ITypes.OutgoingResponse, ITypes.ErrorCode>.ok(response)
                    );
                    _stream = new OutputStream(_responseBody.Write());
                }

                return _stream;
            }
        }

        public PipeWriter Writer
        {
            get
            {
                if (_writer is null)
                {
                    _writer = PipeWriter.Create(Stream);
                }

                return _writer;
            }
        }

        IHeaderDictionary IHttpRequestFeature.Headers { get; set; } = new HeaderDictionary();
        IHeaderDictionary IHttpResponseFeature.Headers { get; set; } = new HeaderDictionary();

        Stream IHttpRequestFeature.Body { get; set; } = default!;
        Stream IHttpResponseFeature.Body
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public async Task CompleteAsync()
        {
            ((IHttpRequestFeature)this).Body.Dispose();

            ITypes.IncomingBody.Finish(_requestBody);

            Stream.Dispose();

            ITypes.OutgoingBody.Finish(_responseBody!, null);

            foreach (var c in _onCompletedCallbacks)
            {
                await c.Item1(c.Item2);
            }
        }

        public void DisableBuffering()
        {
            // We don't buffer anyway
        }

        public void OnCompleted(Func<object, Task> callback, object state)
        {
            _onCompletedCallbacks.Add((callback, state));
        }

        public void OnStarting(Func<object, Task> callback, object state)
        {
            _onStartingCallbacks.Add((callback, state));
        }

        public async Task SendFileAsync(
            string path,
            long offset,
            long? count,
            CancellationToken cancellationToken = default
        )
        {
            using var file = File.OpenRead(path);
            if (offset != 0 || (count is not null && count != file.Length))
            {
                throw new Exception("TODO: handle offset and count");
            }
            await file.CopyToAsync(Stream);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            foreach (var c in _onStartingCallbacks)
            {
                await c.Item1(c.Item2);
            }
        }
    }
}
