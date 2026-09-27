using Microsoft.AspNetCore.Mvc;
using RecordDB.API.Models;
using RecordDB.API.Repositories;

namespace RecordDB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TotalController(ITotalRepository totalRepository) : Controller
    {
        private readonly ITotalRepository _totalRepository = totalRepository;

        [HttpGet]
        public async Task<IActionResult> GetTotals()
        {
            var totals = await _totalRepository.GetTotalCosts();
            return Ok(totals);
        }
    }
}
