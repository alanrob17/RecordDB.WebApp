using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Services
{
    public interface IStatisticService
    {
        Task<StatisticDto?> GetStatisticsAsync();
    }
}
