using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Services
{
    public interface ITotalService
    {
        Task<IEnumerable<TotalDto>> GetTotalCostsAsync();
    }
}
