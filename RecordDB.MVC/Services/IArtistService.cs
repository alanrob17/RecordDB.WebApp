using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient wrapper for the Artist API endpoints.
    /// </summary>
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> GetAllAsync();
        Task<IEnumerable<ArtistDto>> GetArtistListAsync();
        Task<ArtistDto?>             GetByIdAsync(int id);
        Task<IEnumerable<ArtistDto>> SearchAsync(string name);
        Task<IEnumerable<ArtistDto>> GetWithNoBiographyAsync();
        Task<int>                    CreateAsync(CreateArtistDto dto);
        Task                         UpdateAsync(int id, UpdateArtistDto dto);
        Task                         DeleteAsync(int id);
    }
}
