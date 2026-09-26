using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient wrapper for the Artist API endpoints.
    /// </summary>
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> GetAllAsync();
        Task<ArtistDto?>             GetByIdAsync(int id);
        Task<IEnumerable<ArtistDto>> SearchAsync(string name);
        Task<int>                    CreateAsync(CreateArtistDto dto);
        Task                         UpdateAsync(int id, UpdateArtistDto dto);
        Task                         DeleteAsync(int id);
    }
}
