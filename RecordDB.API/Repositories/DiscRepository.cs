using Dapper;
using RecordDB.API.Data;
using RecordDB.Shared.DTOs;
using RecordDB.API.Models;
using System.Data;

namespace RecordDB.API.Repositories
{
    public class DiscRepository(IDataAccess db) : IDiscRepository
    {
        private readonly IDataAccess _db = db;

        public async Task<IEnumerable<ArtistRecordDiscDto>> SelectAllDiscEntitiesAsync()
        {
            string sproc = "up_SelectAllDiscEntities";
            return await _db.GetData<ArtistRecordDiscDto, dynamic>(sproc, new { });
        }

        public async Task<IEnumerable<ArtistRecordDiscDto>> GetDiscRecordsByRecordNameAsync(string recordName)
        {
            if (string.IsNullOrWhiteSpace(recordName))
                return Enumerable.Empty<ArtistRecordDiscDto>();

            string sproc = "up_GetDiscRecordsByRecordName";
            var parameters = new DynamicParameters();
            parameters.Add("@Name", recordName);

            return await _db.GetData<ArtistRecordDiscDto, dynamic>(sproc, parameters);
        }

        public async Task<ArtistRecordDiscDto?> SelectSingleDiscAsync(int discId)
        {
            string sproc = "up_SelectSingleDisc";
            var parameters = new DynamicParameters();
            parameters.Add("@DiscId", discId);

            return await _db.GetFirstOrDefault<ArtistRecordDiscDto, dynamic>(sproc, parameters);
        }

        public async Task<int> InsertDiscAsync(Disc disc)
        {
            var discId = -1;
            string sproc = "up_InsertDisc";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RecordId",     disc.RecordId);
                parameters.Add("@DiscNo",        disc.DiscNo);
                parameters.Add("@FreeDbDiscId",  disc.FreeDbDiscId);
                parameters.Add("@FreeDbId",      disc.FreeDbId);
                parameters.Add("@Length",        disc.Length);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                discId = await _db.SaveDataReturnId(sproc, parameters);
                return discId;
            }
            catch (Exception)
            {
                return discId;
            }
        }

        public async Task<int> UpdateDiscAsync(Disc disc)
        {
            var discId = -1;
            string sproc = "up_UpdateDisc";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@DiscId",        disc.DiscId);
                parameters.Add("@DiscNo",        disc.DiscNo);
                parameters.Add("@FreeDbDiscId",  disc.FreeDbDiscId);
                parameters.Add("@FreeDbId",      disc.FreeDbId);
                parameters.Add("@Length",        disc.Length);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                discId = await _db.SaveDataReturnId(sproc, parameters);
                return discId;
            }
            catch (Exception)
            {
                return discId;
            }
        }

        public async Task DeleteDiscAsync(int discId)
        {
            try
            {
                string sproc = "up_DiscDelete";
                var parameters = new DynamicParameters();
                parameters.Add("@DiscId", discId);

                await _db.SaveData(sproc, parameters);
            }
            catch (Exception)
            {
                // swallow — caller receives 204 regardless
            }
        }

        public async Task UpdateDiscLengthAsync(int discId, int? totalLength)
        {
            string sproc = "up_UpdateDiscLength";
            var parameters = new DynamicParameters();
            parameters.Add("@DiscId",  discId);
            parameters.Add("@Length",  totalLength);

            await _db.SaveData(sproc, parameters);
        }
    }
}
