using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.//////////////
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{

    // Msstransit Outbox -> In case RabbitMQ is not running, an event that was not consummed by RabbitMQ will be inserted into this outbox and soon as RabbitMQ is back on the event in the outbox will be consummed by RabbitMQ
    x.AddEntityFrameworkOutbox<AuctionDbContext>(options =>
    {
        // When RabbitMq is back up, this will query the outbox every 10 seconds to look for events that were not consumed by RabbitMQ
        options.QueryDelay = TimeSpan.FromSeconds(10);
        options.UsePostgres();
        options.UseBusOutbox();

    });

    // Rabbit will connect to localhost by default
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
///////////////////////////////////////////////
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}


app.Run();
