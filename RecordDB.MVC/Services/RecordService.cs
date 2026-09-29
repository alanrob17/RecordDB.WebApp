using RecordDB.Shared.DTOs;
using System.Net.Http.Json;

namespace RecordDB.MVC.Services
{
    /// <summary>
    /// Typed HttpClient implementation that calls the RecordDB.API record endpoints.
    /// </summary>
    public class RecordService(HttpClient http) : IRecordService
    {
        // -----------------------------------------------------------------------
        // GET
        // -----------------------------------------------------------------------

        public async Task<IEnumerable<ArtistRecordDto>> GetAllAsync()
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDto>>("api/record") ?? [];

        public async Task<ArtistRecordDto?> GetByIdAsync(int id)
            => await http.GetFromJsonAsync<ArtistRecordDto>($"api/record/{id}");

        public async Task<IEnumerable<ArtistRecordDto>> GetRecordsShowAsync(string show)
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDto>>($"api/record/show/{Uri.EscapeDataString(show)}") ?? [];

        public async Task<IEnumerable<ArtistRecordDto>> GetByArtistNameAsync(string name)
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDto>>($"api/record/by-artist/{Uri.EscapeDataString(name)}") ?? [];

        public async Task<IEnumerable<ArtistRecordDto>> GetByYearAsync(int year)
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDto>>($"api/record/by-year/{year}") ?? [];

        public async Task<IEnumerable<ArtistRecordDto>> GetRecordReviewsAsync()
            => await http.GetFromJsonAsync<IEnumerable<ArtistRecordDto>>("api/record/record-reviews") ?? [];

        // -----------------------------------------------------------------------
        // POST
        // -----------------------------------------------------------------------

        public async Task<int> CreateAsync(CreateRecordDto dto)
        {
            var response = await http.PostAsJsonAsync("api/record", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        // -----------------------------------------------------------------------
        // PUT
        // -----------------------------------------------------------------------

        public async Task UpdateAsync(int id, UpdateRecordDto dto)
        {
            var response = await http.PutAsJsonAsync($"api/record/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        public async Task DeleteAsync(int id)
        {
            var response = await http.DeleteAsync($"api/record/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
