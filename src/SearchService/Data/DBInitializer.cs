using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data
{
    public class DBInitializer
    {

        public static async Task InitDb(WebApplication app)
        {

            // Create the MongoDB connection
            await DB.InitAsync("SearchDB", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDBConnection")));

            // Create the MongoDB Indexes
            await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();

            // Check how many item are in the mongo database
            var count = await DB.CountAsync<Item>();

            using var scope = app.Services.CreateScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

            var items = await httpClient.GetItemsForSearchDb();

            Console.WriteLine("Items from Auction Service: " + items.Count);

            if (items.Count > 0) await DB.SaveAsync(items);
        }

    }
}