using RecordDB.API.Models;

namespace RecordDB.API.Repositories
{
    public interface ITotalRepository
    {
        Task<IEnumerable<TotalDto>> GetTotalCosts();
    }
}
