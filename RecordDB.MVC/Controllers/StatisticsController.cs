using Microsoft.AspNetCore.Mvc;
using RecordDB.MVC.Services;
using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Controllers
{
    /// <summary>
    /// MVC controller for displaying database statistics.
    /// Data is retrieved via <see cref="IStatisticService"/> (typed HttpClient → RecordDB.API).
    /// </summary>
    public class StatisticsController(IStatisticService statisticService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var statistics = await statisticService.GetStatisticsAsync();
            return View(statistics ?? new StatisticDto());
        }
    }
}
