using Microsoft.AspNetCore.Mvc;
using RecordDB.MVC.Models;
using RecordDB.MVC.Services;
using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Controllers
{
    /// <summary>
    /// MVC controller for Total cost operations.
    /// All data access goes through <see cref="ITotalService"/>.
    /// </summary>
    public class TotalController(ITotalService totalService) : Controller
    {
        // -----------------------------------------------------------------------
        // Index — redirect to TotalCosts
        // -----------------------------------------------------------------------

        public IActionResult Index()
        {
            return RedirectToAction(nameof(TotalCosts));
        }

        // -----------------------------------------------------------------------
        // TotalCosts — paginated list of artist total disc counts and costs
        // -----------------------------------------------------------------------

        public async Task<IActionResult> TotalCosts(int page = 1)
        {
            const int pageSize = 20;

            var all = (await totalService.GetTotalCostsAsync()).ToList();

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Clamp page to valid range
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var vm = new PaginatedViewModel<TotalDto>
            {
                Items       = all.Skip((page - 1) * pageSize).Take(pageSize),
                CurrentPage = page,
                TotalPages  = totalPages,
                TotalCount  = totalCount,
                PageSize    = pageSize
            };

            // Summary totals for the header cards
            ViewBag.GrandTotalDiscs = all.Sum(t => t.TotalDiscs);
            ViewBag.GrandTotalCost  = all.Sum(t => t.TotalCost);

            return View("~/Views/Total/TotalCosts.cshtml", vm);
        }
    }
}
