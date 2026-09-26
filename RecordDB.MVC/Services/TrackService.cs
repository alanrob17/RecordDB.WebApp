using RecordDB.Shared.DTOs;
using System.Net.Http.Json;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient implementation that calls the RecordDB.API track endpoints.
    /// </summary>
    public class TrackService(HttpClient http) : ITrackService
    {
        // -----------------------------------------------------------------------
        // GET
        // -----------------------------------------------------------------------

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> GetAllAsync()
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDiscTrackDto>>("api/track") ?? [];

        public async Task<ArtistRecordDiscTrackDto?> GetByIdAsync(int id)
            => await http.GetFromJsonAsync<ArtistRecordDiscTrackDto>($"api/track/{id}");

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> SearchByNameAsync(string name)
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDiscTrackDto>>($"api/track/search/{Uri.EscapeDataString(name)}") ?? [];

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> GetByArtistAsync(string name)
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDiscTrackDto>>($"api/track/by-artist/{Uri.EscapeDataString(name)}") ?? [];

        public async Task<IEnumerable<ArtistRecordDiscTrackDto>> GetByRecordAsync(string name)
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDiscTrackDto>>($"api/track/by-record/{Uri.EscapeDataString(name)}") ?? [];

        // -----------------------------------------------------------------------
        // POST — single
        // -----------------------------------------------------------------------

        public async Task<int> CreateAsync(CreateTrackDto dto)
        {
            var response = await http.PostAsJsonAsync("api/track", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        // -----------------------------------------------------------------------
        // POST — bulk
        // -----------------------------------------------------------------------

        public async Task BulkCreateAsync(IEnumerable<CreateTrackDto> dtos)
        {
            var response = await http.PostAsJsonAsync("api/track/bulk", dtos);
            response.EnsureSuccessStatusCode();
        }

        // -----------------------------------------------------------------------
        // PUT
        // -----------------------------------------------------------------------

        public async Task UpdateAsync(int id, UpdateTrackDto dto)
        {
            var response = await http.PutAsJsonAsync($"api/track/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        public async Task DeleteAsync(int id)
        {
            var response = await http.DeleteAsync($"api/track/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
