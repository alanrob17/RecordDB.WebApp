using RecordDB.API.Models;

namespace RecordDB.API.Repositories
{
    public interface IStatisticRepository
    {
        Task<Statistic> GetStatisticsAsync();
    }
}
