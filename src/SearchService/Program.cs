using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.//////////////

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetRetryPolicy());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search-service", false));
    // Rabbit will connect to localhost by default
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-service-auction-created", e =>
        {
            // Configure retry policy for Auction created consumer
            e.UseMessageRetry(r => r.Interval(5, 5));

            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});
///////////////////////////////////////////////


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
