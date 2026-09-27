using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Services
{
    public class TotalService(HttpClient http) : ITotalService
    {
        public async Task<IEnumerable<TotalDto>> GetTotalCostsAsync()
            => await http.GetFromJsonAsync<IEnumerable<TotalDto>>("api/total") ?? [];
    }
}
