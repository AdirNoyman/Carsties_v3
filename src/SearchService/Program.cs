using Polly;
using Polly.Extensions.Http;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetRetryPolicy());


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DBInitializer.InitDb(app);
        Console.WriteLine("Search Service has started 🚀");
    }
    catch (Exception e)
    {

        Console.WriteLine("Error initializing Search Service database 😫: " + e.Message);
    }
});

app.Run();

// Handler for the HTTP client in case of transient errors
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    // In the event of a transient error (5xx, 408, etc), Polly will wait 3 seconds before retrying the request. And will retry forever (every 3 seconds).
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
}
