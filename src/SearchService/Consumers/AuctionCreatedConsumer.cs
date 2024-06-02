using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            var message = context.Message;
            var item = _mapper.Map<Item>(context.Message);
            Console.WriteLine($"Consuming auction created: {message.Id}");

            // Example of an catching an error just for learning purposes
            if (item.Model == "Reno") throw new ArgumentException("Reno is not allowed");

            await item.SaveAsync();
        }
    }

}