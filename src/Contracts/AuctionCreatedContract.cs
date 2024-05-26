using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts
{
    public class AuctionCreatedContract
    {
        public Guid Id { get; set; }
        public int ReservePrice { get; set; }

        public string Seller { get; set; }

        public string Winner { get; set; }

        public int SoldAmount { get; set; }

        public int CurrentHighestBid { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime AuctionEndsAt { get; set; }

        public string Status { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public string Color { get; set; }

        public int Mileage { get; set; }

        public string ImageUrl { get; set; }



    }
}