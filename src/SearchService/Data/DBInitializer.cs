using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

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

            if (count == 0)
            {

                Console.WriteLine("No data in the database - attempting to seed the database 🤓");
                var itemData = await File.ReadAllTextAsync("Data/auctions.json");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // Convert the list of items inside the JSON file into an array of C# Items
                var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

                await DB.SaveAsync(items);

            }
        }

    }
}