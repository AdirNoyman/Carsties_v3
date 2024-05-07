using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthorization();

app.MapControllers();

// Create the MongoDB connection
await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDBConnection")));

// Create the MongoDB Indexes
await DB.Index<Item>()
.Key(x => x.Make, KeyType.Text)
.Key(x => x.Model, KeyType.Text)
.Key(x => x.Color, KeyType.Text)
.CreateAsync();



app.Run();
