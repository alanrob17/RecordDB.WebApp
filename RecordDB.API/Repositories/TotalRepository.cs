using RecordDB.API.Data;
using RecordDB.API.Models;

namespace RecordDB.API.Repositories
{
    public class TotalRepository(IDataAccess db) : ITotalRepository
    {
        private readonly IDataAccess _db = db;

        public async Task<IEnumerable<TotalDto>> GetTotalCosts()
        {
            var sproc = "sp_getTotalsForEachArtist";

            var totals = await _db.GetData<TotalDto, dynamic>(sproc, new { });

            return totals.ToList();
        }
    }
}
