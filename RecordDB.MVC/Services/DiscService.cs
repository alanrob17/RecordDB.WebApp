using RecordDB.Shared.DTOs;
using System.Net.Http.Json;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient implementation that calls the RecordDB.API disc endpoints.
    /// </summary>
    public class DiscService(HttpClient http) : IDiscService
    {
        // -----------------------------------------------------------------------
        // GET
        // -----------------------------------------------------------------------

        public async Task<IEnumerable<ArtistRecordDiscDto>> GetAllAsync()
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDiscDto>>("api/disc") ?? [];

        public async Task<ArtistRecordDiscDto?> GetByIdAsync(int id)
            => await http.GetFromJsonAsync<ArtistRecordDiscDto>($"api/disc/{id}");

        public async Task<IEnumerable<ArtistRecordDiscDto>> SearchByRecordNameAsync(string name)
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDiscDto>>($"api/disc/search/{Uri.EscapeDataString(name)}") ?? [];

        // -----------------------------------------------------------------------
        // POST
        // -----------------------------------------------------------------------

        public async Task<int> CreateAsync(CreateDiscDto dto)
        {
            var response = await http.PostAsJsonAsync("api/disc", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        // -----------------------------------------------------------------------
        // PUT
        // -----------------------------------------------------------------------

        public async Task UpdateAsync(int id, UpdateDiscDto dto)
        {
            var response = await http.PutAsJsonAsync($"api/disc/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        // -----------------------------------------------------------------------
        // PATCH — length only
        // -----------------------------------------------------------------------

        public async Task UpdateLengthAsync(int id, int? length)
        {
            var response = await http.PatchAsJsonAsync($"api/disc/{id}/length", new UpdateDiscLengthDto { Length = length });
            response.EnsureSuccessStatusCode();
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        public async Task DeleteAsync(int id)
        {
            var response = await http.DeleteAsync($"api/disc/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
