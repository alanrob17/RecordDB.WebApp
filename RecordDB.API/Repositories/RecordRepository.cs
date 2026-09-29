using Dapper;
using RecordDB.API.Data;
using RecordDB.Shared.DTOs;
using RecordDB.API.Extensions;
using RecordDB.API.Models;
using System.Data;
using System.Globalization;

namespace RecordDB.API.Repositories
{
    public class RecordRepository(IDataAccess db) : IRecordRepository
    {
        private readonly IDataAccess _db = db;

        public async Task<ArtistRecordDto?> SelectAsync(int recordId)
        {
            var sproc = "up_RecordSelectByIdCore";
            var parameter = new DynamicParameters();
            parameter.Add("@RecordId", recordId);
            return await _db.GetFirstOrDefault<ArtistRecordDto, dynamic>(sproc, parameter);
        }

        public async Task<List<ArtistRecordDto>> SelectAsync()
        {
            var sproc = "up_RecordSelectAll";
            var records = await _db.GetData<ArtistRecordDto, dynamic>(sproc, new { });
            return records.ToList();
        }

        public async Task<List<Record>> SelectByShowAsync(string show)
        {
            ArgumentNullException.ThrowIfNull(show);

            var sproc = "up_RecordSelectShowCore";
            var parameter = new DynamicParameters();
            parameter.Add("@Show", show);
            var records = await _db.GetData<Record, dynamic>(sproc, parameter);
            return records.ToList();
        }

        public async Task<List<ArtistRecordDto>> SelectRecordsShowAsync(string show)
        {
            if (string.IsNullOrWhiteSpace(show))
                throw new ArgumentNullException(nameof(show));

            var sproc = "up_RecordSelectShowCore";
            var parameter = new DynamicParameters();
            parameter.Add("@Show", show);
            var records = await _db.GetData<ArtistRecordDto, dynamic>(sproc, parameter);
            return records.ToList();
        }

        public async Task<List<Record>> GetArtistRecordsAsync(int artistId)
        {
            var sproc = "up_getArtistRecords";

            var records = await _db.GetData<Record, Artist, Record>(
                sproc,
                (record, artist) =>
                {
                    record.ArtistId = artist.ArtistId;
                    return record;
                },
                new { ArtistId = artistId },
                splitOn: "ArtistId");

            return records.ToList();
        }

        public async Task<List<Record>> SelectArtistRecordsAsync(int artistId)
        {
            var sproc = "up_getRecordListAndNone";
            var parameter = new DynamicParameters();
            parameter.Add("@ArtistId", artistId);
            var records = await _db.GetData<Record, dynamic>(sproc, parameter);
            return records.ToList();
        }

        public async Task<List<RecordReviewDto>> SelectRecordReviewsAsync()
        {
            var sproc = "up_SelectRecordReviewsCore";
            var records = await _db.GetData<RecordReviewDto, dynamic>(sproc, new { });
            return records.ToList();
        }

        public async Task<List<Record>> SelectRecordReviews()
        {
            var sproc = "up_SelectRecordReviews2";
            var records = (await _db.GetData<Record, dynamic>(sproc, new { })).ToList();

            var allRecords = await SelectAsync();
            var lookup = allRecords
                .Where(r => !string.IsNullOrEmpty(r.Name) && !string.IsNullOrEmpty(r.ArtistName))
                .GroupBy(r => (r.ArtistName!.Trim().ToLowerInvariant(), r.Name!.Trim().ToLowerInvariant()))
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var r in records)
            {
                var artistName = r.ArtistName ?? r.Artist?.Name ?? string.Empty;
                var recordName = r.Name ?? string.Empty;
                var key = (artistName.Trim().ToLowerInvariant(), recordName.Trim().ToLowerInvariant());

                if (lookup.TryGetValue(key, out var match))
                {
                    r.RecordId = match.RecordId;
                    r.ArtistId = match.ArtistId;
                }

                if (r.Artist == null)
                {
                    r.Artist = new Artist { ArtistId = r.ArtistId, Name = artistName };
                }
            }

            return records;
        }

        public async Task<List<ArtistRecordDto>> GetRecordsByArtistNameAsync(string artistName)
        {
            if (string.IsNullOrWhiteSpace(artistName))
                return [];

            var sproc = "up_GetRecordsByArtistName";
            var parameter = new DynamicParameters();
            parameter.Add("@ArtistName", artistName);
            var records = await _db.GetData<ArtistRecordDto, dynamic>(sproc, parameter);
            return records.ToList();
        }

        public async Task<List<ArtistRecordDto>> GetRecordsByYearAsync(int recorded)
        {
            var sproc = "up_GetRecordsByYear";
            var parameter = new DynamicParameters();
            parameter.Add("@Recorded", recorded);
            var records = await _db.GetData<ArtistRecordDto, dynamic>(sproc, parameter);
            return records.ToList();
        }

        public async Task<List<Total>> GetTotalCostsAsync()
        {
            var sproc = "sp_getTotalsForEachArtist";
            var totals = await _db.GetData<Total, dynamic>(sproc, new { });
            return totals.ToList();
        }

        public async Task<List<ArtistRecordDiscDto>> ListRecordsWithNoTracksAsync()
        {
            var sproc = "up_GetRecordsWithNoTracks";
            var records = await _db.GetData<ArtistRecordDiscDto, dynamic>(sproc, new { });
            return records.ToList();
        }

        public async Task<List<ArtistRecordDiscDto>> ArtistRecordsWithNoTracksAsync(string name)
        {
            var sproc = "up_GetArtistRecordsWithNoTracks";
            var parameter = new DynamicParameters();
            parameter.Add("@Name", name);
            var records = await _db.GetData<ArtistRecordDiscDto, dynamic>(sproc, parameter);
            return records.ToList();
        }

        public async Task<List<ArtistRecordDiscTrackDto>> SelectTracksByPartialNameAsync(string name)
        {
            var sproc = "up_SelectPartialRecordTracks";
            var parameter = new DynamicParameters();
            parameter.Add("@Name", name);
            var records = await _db.GetData<ArtistRecordDiscTrackDto, dynamic>(sproc, parameter);
            return records.ToList();
        }

        public async Task<List<MissingReviewDto>> NoRecordReviewsAsync()
        {
            var sproc = "up_NoRecordReviews";
            var records = await _db.GetData<MissingReviewDto, dynamic>(sproc, new { });
            return records.ToList();
        }

        public async Task<string> CountDiscsAsync(string show)
        {
            ArgumentNullException.ThrowIfNull(show);

            var sproc = "up_CountDiscs";
            var parameter = new DynamicParameters();
            parameter.Add("@Show", show);
            var discs = await _db.GetScalar<int, object>(sproc, parameter);
            return discs.ToString(CultureInfo.InvariantCulture);
        }

        public async Task<string> GetArtistNumberOfRecordsAsync(int artistId)
        {
            var sproc = "up_GetArtistNumberOfRecords";
            var parameter = new DynamicParameters();
            parameter.Add("@ArtistId", artistId);
            var count = await _db.GetScalar<int, object>(sproc, parameter);
            return count.ToString(CultureInfo.InvariantCulture);
        }

        public async Task<string> GetRecordedYearNumberAsync(int year)
        {
            var sproc = "up_GetRecordedYearNumber";
            var parameter = new DynamicParameters();
            parameter.Add("@Year", year);
            var count = await _db.GetScalar<int, dynamic>(sproc, parameter);
            return count.ToString(CultureInfo.InvariantCulture);
        }

        public async Task<int> InsertAsync(Record record)
        {
            var recordId = -1;
            var sproc = "adm_RecordInsert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ArtistId",  record.ArtistId);
                parameters.Add("@Name",      record.Name);
                parameters.Add("@Field",     record.Field);
                parameters.Add("@Recorded",  record.Recorded);
                parameters.Add("@Label",     record.Label);
                parameters.Add("@Pressing",  record.Pressing);
                parameters.Add("@Rating",    record.Rating);
                parameters.Add("@Discs",     record.Discs);
                parameters.Add("@Media",     record.Media);
                parameters.Add("@Bought",    record.Bought);
                parameters.Add("@Cost",      record.Cost);
                parameters.Add("@CoverName", record.CoverName);
                parameters.Add("@Review",    record.Review);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                recordId = await _db.SaveDataReturnId(sproc, parameters);
                return recordId;
            }
            catch (Exception)
            {
                return recordId;
            }
        }

        public async Task<int> UpdateAsync(ArtistRecordDto record)
        {
            var recordId = 0;

            try
            {
                string sproc = "adm_UpdateRecord";
                var parameters = new DynamicParameters();
                parameters.Add("@RecordId",  record.RecordId);
                parameters.Add("@ArtistId",  record.ArtistId);
                parameters.Add("@Name",      record.Name);
                parameters.Add("@Field",     record.Field);
                parameters.Add("@Recorded",  record.Recorded);
                parameters.Add("@Label",     record.Label);
                parameters.Add("@Pressing",  record.Pressing);
                parameters.Add("@Rating",    record.Rating);
                parameters.Add("@Discs",     record.Discs);
                parameters.Add("@Media",     record.Media);
                parameters.Add("@Bought",    record.Bought);
                parameters.Add("@Cost",      record.Cost);
                parameters.Add("@Review",    record.Review);

                await _db.SaveData(sproc, parameters);
                recordId = record.RecordId;
                return recordId;
            }
            catch (Exception)
            {
                return recordId;
            }
        }

        public async Task DeleteAsync(int recordId)
        {
            try
            {
                var sproc = "up_deleteRecord";
                var parameter = new DynamicParameters();
                parameter.Add("@RecordId", recordId);
                await _db.SaveData(sproc, parameter);
            }
            catch (Exception)
            {
                // swallow — caller receives 204 regardless
            }
        }
    }
}
