using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient wrapper for the Disc API endpoints.
    /// </summary>
    public interface IDiscService
    {
        Task<IEnumerable<ArtistRecordDiscDto>> GetAllAsync();
        Task<ArtistRecordDiscDto?>             GetByIdAsync(int id);
        Task<IEnumerable<ArtistRecordDiscDto>> SearchByRecordNameAsync(string name);
        Task<int>                              CreateAsync(CreateDiscDto dto);
        Task                                   UpdateAsync(int id, UpdateDiscDto dto);
        Task                                   UpdateLengthAsync(int id, int? length);
        Task                                   DeleteAsync(int id);
    }
}
