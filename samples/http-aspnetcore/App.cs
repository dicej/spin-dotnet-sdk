using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spin.Http;
using SpinHttpWorld.wit.imports.wasi.http.v0_2_0;

namespace SpinHttpWorld.wit.exports.wasi.http.v0_2_0;

public class IncomingHandlerImpl : IIncomingHandler
{
    /// <summary>Handle the specified incoming HTTP request and send a response
    /// via `responseOut`.</summary>
    public static void Handle(ITypes.IncomingRequest request, ITypes.ResponseOutparam responseOut)
    {
        var builder = WebApplication.CreateBuilder(new string[0]);
        builder.Services.AddSingleton<IServer, WasiHttpServer>();
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new WasiLoggingProvider()).AddFilter("Microsoft.AspNetCore.DataProtection", LogLevel.Error);
        builder.Services.AddRazorPages();

        var app = builder.Build();
        app.UseStaticFiles();
        app.MapRazorPages();

        app.MapGet("/", () => "Hello, world! See also: /weatherforecast and /myrazorpage");

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
                    .Select(index => new
                    {
                        Date = DateTime.Now.AddDays(index),
                        TempC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            }
        );

        Func<Task> task = async () =>
        {
            await app.StartAsync();
            await WasiHttpServer.HandleRequestAsync(request, responseOut);
        };
        RequestHandler.Run(task());
    }
}
