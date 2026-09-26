using Microsoft.AspNetCore.Mvc;
using RecordDB.API.Models;
using RecordDB.API.Repositories;
using RecordDB.Shared.DTOs;

namespace RecordDB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StatisticController(IStatisticRepository statisticRepository) : ControllerBase
    {
        private static StatisticDto ToDto(Statistic stats) => new()
        {
            TotalCDs        = stats.TotalCDs,
            TotalRecords    = stats.TotalRecords,
            TotalCost       = stats.TotalCost,
            RecordCost      = stats.RecordCost,
            CDCost          = stats.CDCost,
            AvCDCost        = stats.AvCDCost,
            RockDisks       = stats.RockDisks,
            FolkDisks       = stats.FolkDisks,
            AcousticDisks   = stats.AcousticDisks,
            JazzDisks       = stats.JazzDisks,
            BluesDisks      = stats.BluesDisks,
            CountryDisks    = stats.CountryDisks,
            ClassicalDisks  = stats.ClassicalDisks,
            SoundtrackDisks = stats.SoundtrackDisks,
            FourStarDisks   = stats.FourStarDisks,
            ThreeStarDisks  = stats.ThreeStarDisks,
            TwoStarDisks    = stats.TwoStarDisks,
            OneStarDisks    = stats.OneStarDisks,
            Disks2017       = stats.Disks2017,
            Cost2017        = stats.Cost2017,
            Av2017          = stats.Av2017,
            Disks2018       = stats.Disks2018,
            Cost2018        = stats.Cost2018,
            Av2018          = stats.Av2018,
            Disks2019       = stats.Disks2019,
            Cost2019        = stats.Cost2019,
            Av2019          = stats.Av2019,
            Disks2020       = stats.Disks2020,
            Cost2020        = stats.Cost2020,
            Av2020          = stats.Av2020,
            Disks2021       = stats.Disks2021,
            Cost2021        = stats.Cost2021,
            Av2021          = stats.Av2021,
            Disks2022       = stats.Disks2022,
            Cost2022        = stats.Cost2022,
            Av2022          = stats.Av2022
        };

        /// <summary>Returns database statistics.</summary>
        /// <response code="200">Database statistics summary.</response>
        [HttpGet]
        [ProducesResponseType(typeof(StatisticDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<StatisticDto>> GetStatistics()
        {
            var statistics = await statisticRepository.GetStatisticsAsync();
            return Ok(ToDto(statistics));
        }
    }
}
