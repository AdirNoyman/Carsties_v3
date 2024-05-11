using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<Item>>> searchItems(string searchTerm, int pageNumber = 1, int pageSize = 4)
        {

            var query = DB.PagedSearch<Item>();
            query.Sort(x => x.Ascending(a => a.Make));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Each search result item will get a score for how much it is closest to the search term
                query.Match(Search.Full, searchTerm).SortByTextScore();
            }


            query.PageNumber(pageNumber);
            query.PageSize(pageSize);

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