using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spin.Http;
using SpinHttpWorld.wit.imports.wasi.http.v0_2_1;

namespace SpinHttpWorld.wit.exports.wasi.http.v0_2_1;

public class IncomingHandlerImpl : IIncomingHandler
{
    /// <summary>Handle the specified incoming HTTP request and send a response
    /// via `responseOut`.</summary>
    public static void Handle(ITypes.IncomingRequest request, ITypes.ResponseOutparam responseOut)
    {
        var builder = WebApplication.CreateSlimBuilder(new string[0]);
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(
                0,
                AppJsonSerializerContext.Default
            );
        });
        builder.Services.AddSingleton<IServer, WasiHttpServer>();
        builder.Logging.ClearProviders();
        builder
            .Logging.AddProvider(new WasiLoggingProvider())
            .AddFilter("Microsoft.AspNetCore.DataProtection", LogLevel.Error);

        var app = builder.Build();
        app.UseStaticFiles();

        app.MapGet("/", () => "Hello, world! See also: /weatherforecast and /mystaticpage.html");

        app.MapGet(
            "/weatherforecast",
            () =>
            {
                var summaries = new[]
                {
                    "Freezing",
                    "Bracing",
                    "Chilly",
                    "Cool",
                    "Mild",
                    "Warm",
                    "Balmy",
                    "Hot",
                    "Sweltering",
                    "Scorching"
                };
                var forecast = Enumerable
                    .Range(1, 5)
                    .Select(index => new Forecast()
                    {
                        Date = DateTime.Now.AddDays(index),
                        TempC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            }
        );

        app.MapPost(
            "/echo",
            async context =>
            {
                context.Response.Headers.ContentType = context.Request.Headers.ContentType;
                await context.Request.Body.CopyToAsync(context.Response.Body);
            }
        );

        app.MapPost("/data", (Message message) => message.Greeting);

        Func<Task> task = async () =>
        {
            await app.StartAsync();
            await WasiHttpServer.HandleRequestAsync(request, responseOut);
        };
        RequestHandler.Run(task());
    }
}

public class Message
{
    public string Greeting { get; set; }
}

public class Forecast
{
    public DateTime Date { get; set; }
    public int TempC { get; set; }
    public string Summary { get; set; }
}

[JsonSerializable(typeof(Forecast[]))]
[JsonSerializable(typeof(Message))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
