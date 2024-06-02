using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;

        public AuctionUpdatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            // Message is the object that was sent to the queue
            var message = context.Message;
            var item = _mapper.Map<Item>(context.Message);
            Console.WriteLine($"Consuming auction updated: {message.Id}");

            var result = await DB.Update<Item>()
            .Match(a => a.ID == context.Message.Id)
            .ModifyOnly(x => new
            {
                x.Color,
                x.Make,
                x.Model,
                x.Year,
                x.Mileage,

            }, item)
            .ExecuteAsync();

            if (!result.IsAcknowledged)
                throw new MessageException(typeof(AuctionUpdated), "Problem updating mongo db 😫");
        }


    }

}