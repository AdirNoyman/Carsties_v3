using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        // The "context" parameter contains the message that was sent
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            Console.WriteLine($"Consuming faulty auction creation: {context.Message.Message.Id}");

            var exception = context.Message.Exceptions.First();

            if (exception.ExceptionType == "System.ArgumentException")
            {
                // Change the model name to "Koko Loko" and publish the message again
                context.Message.Message.Model = "Koko Loko";
                await context.Publish(context.Message.Message);
            }
            else
            {
                // Log the exception
                Console.WriteLine($"Unknown Exception: {exception.Message}");
            }

        }
    }
}