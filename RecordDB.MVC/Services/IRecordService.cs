using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient wrapper for the Record API endpoints.
    /// </summary>
    public interface IRecordService
    {
        Task<IEnumerable<ArtistRecordDto>> GetAllAsync();
        Task<ArtistRecordDto?>             GetByIdAsync(int id);
        Task<IEnumerable<ArtistRecordDto>> GetRecordsShowAsync(string show);
        Task<IEnumerable<ArtistRecordDto>> GetByArtistNameAsync(string name);
        Task<IEnumerable<ArtistRecordDto>> GetByYearAsync(int year);
        Task<int>                          CreateAsync(CreateRecordDto dto);
        Task                               UpdateAsync(int id, UpdateRecordDto dto);
        Task                               DeleteAsync(int id);
    }
}
