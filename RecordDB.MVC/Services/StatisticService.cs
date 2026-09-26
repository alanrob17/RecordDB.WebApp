using RecordDB.Shared.DTOs;
using System.Net.Http.Json;

namespace RecordDB.MVC.Services
{
    public class StatisticService(HttpClient http) : IStatisticService
    {
        public async Task<StatisticDto?> GetStatisticsAsync()
        {
            var response = await http.GetAsync("api/statistic");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StatisticDto>();
        }
    }
}
