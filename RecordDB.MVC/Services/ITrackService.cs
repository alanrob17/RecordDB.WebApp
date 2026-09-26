using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient wrapper for the Track API endpoints.
    /// </summary>
    public interface ITrackService
    {
        Task<IEnumerable<ArtistRecordDiscTrackDto>> GetAllAsync();
        Task<ArtistRecordDiscTrackDto?>             GetByIdAsync(int id);
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SearchByNameAsync(string name);
        Task<IEnumerable<ArtistRecordDiscTrackDto>> GetByArtistAsync(string name);
        Task<IEnumerable<ArtistRecordDiscTrackDto>> GetByRecordAsync(string name);
        Task<int>                                    CreateAsync(CreateTrackDto dto);
        Task                                         BulkCreateAsync(IEnumerable<CreateTrackDto> dtos);
        Task                                         UpdateAsync(int id, UpdateTrackDto dto);
        Task                                         DeleteAsync(int id);
    }
}
