using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sparrow.Json;

namespace Reviews.Service.QueryApi.Modules.Reviews
{
    [Route("/reviews")]
    [ApiController]
    public class QueryApi
    {
        private readonly ReviewQueryService reviewQueryService;

        public QueryApi(ReviewQueryService queryService)=>  reviewQueryService = queryService;
        
        [Route("status")]
        [HttpGet]
        public async Task<IActionResult> Get() => new OkResult();

    }
}
