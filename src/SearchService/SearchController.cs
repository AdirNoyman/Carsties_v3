using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<Item>>> searchItems([FromQuery] SearchParams searchParams)
        {

            var query = DB.PagedSearch<Item, Item>();


            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                // Each search result item will get a score for how much it is closest to the search term
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
            }

            // Sort ///////////////////////////
            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make)),
                "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEndsAt)),
            };

            // Filter ///////////////////////////
            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEndsAt < DateTime.UtcNow),
                "endingSoon" => query.Match(x => x.AuctionEndsAt < DateTime.UtcNow.AddHours(6) && x.AuctionEndsAt > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEndsAt > DateTime.UtcNow)
            };

            // Find auctions by seller
            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query.Match(x => x.Seller == searchParams.Seller);
            }


            // Find auctions by winner
            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query.Match(x => x.Winner == searchParams.Winner);
            }

            query.PageNumber(searchParams.PageNumber);
            query.PageSize(searchParams.PageSize);

            var result = await query.ExecuteAsync();

            return Ok(new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount

            });

        }

    }
}