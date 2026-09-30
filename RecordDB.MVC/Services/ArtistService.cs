using RecordDB.Shared.DTOs;
using System.Net.Http.Json;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient implementation that calls the RecordDB.API artist endpoints.
    /// </summary>
    public class ArtistService(HttpClient http) : IArtistService
    {
        // -----------------------------------------------------------------------
        // GET
        // -----------------------------------------------------------------------

        public async Task<IEnumerable<ArtistDto>> GetAllAsync()
            => await http.GetFromJsonAsync<IEnumerable<ArtistDto>>("api/artist") ?? [];

        public async Task<IEnumerable<ArtistDto>> GetArtistListAsync()
            => await http.GetFromJsonAsync<IEnumerable<ArtistDto>>("api/artist/artist-list") ?? [];

        public async Task<ArtistDto?> GetByIdAsync(int id)
            => await http.GetFromJsonAsync<ArtistDto>($"api/artist/{id}");

        public async Task<IEnumerable<ArtistDto>> SearchAsync(string name)
            => await http.GetFromJsonAsync<IEnumerable<ArtistDto>>($"api/artist/search/{Uri.EscapeDataString(name)}") ?? [];

        public async Task<IEnumerable<ArtistDto>> GetWithNoBiographyAsync()
            => await http.GetFromJsonAsync<IEnumerable<ArtistDto>>("api/artist/no-biography") ?? [];

        // -----------------------------------------------------------------------
        // POST
        // -----------------------------------------------------------------------

        public async Task<int> CreateAsync(CreateArtistDto dto)
        {
            var response = await http.PostAsJsonAsync("api/artist", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        // -----------------------------------------------------------------------
        // PUT
        // -----------------------------------------------------------------------

        public async Task UpdateAsync(int id, UpdateArtistDto dto)
        {
            var response = await http.PutAsJsonAsync($"api/artist/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        public async Task DeleteAsync(int id)
        {
            var response = await http.DeleteAsync($"api/artist/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
