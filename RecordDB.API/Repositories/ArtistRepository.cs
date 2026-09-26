using Dapper;
using RecordDB.API.Data;
using RecordDB.API.Models;
using System.Data;

namespace RecordDB.API.Repositories
{
    public class ArtistRepository(IDataAccess db) : IArtistRepository
    {
        private readonly IDataAccess _db = db;

        public async Task<IEnumerable<Artist>> GetArtistsAsync()
        {
            string sproc = "up_ArtistSelectFull";
            return await _db.GetData<Artist, dynamic>(sproc, new { });
        }

        public async Task<IEnumerable<Artist>> GetArtistsByPartialNameAsync(string name)
        {
            string sproc = "up_GetArtistsByPartialName";
            var parameter = new DynamicParameters();
            parameter.Add("@Name", name);
            IEnumerable<Artist> artists = await _db.GetData<Artist, dynamic>(sproc, parameter);
            return artists.ToList();
        }

        public async Task<List<Artist>> SelectAsync()
        {
            string sproc = "up_ArtistSelectAll";
            IEnumerable<Artist> artists = await _db.GetData<Artist, dynamic>(sproc, new { });
            return artists.ToList();
        }

        public async Task<Artist?> SelectAsync(int artistId)
        {
            string sproc = "up_ArtistSelectById";
            var parameter = new DynamicParameters();
            parameter.Add("@ArtistId", artistId);
            return await _db.GetFirstOrDefault<Artist, dynamic>(sproc, parameter);
        }

        public async Task<IEnumerable<Artist>> GetArtistsWithNoBiographyAsync()
        {
            string sproc = "up_SelectArtistsWithNoBiography";
            return await _db.GetData<Artist, dynamic>(sproc, new { });
        }

        public async Task<Artist?> GetArtistWithNoBiographyAsync(string name)
        {
            string sproc = "up_SearchForArtistWithNoBiography";
            var parameter = new DynamicParameters();
            parameter.Add("@Name", name);
            return await _db.GetFirstOrDefault<Artist, dynamic>(sproc, parameter);
        }

        public async Task<int> InsertAsync(Artist artist)
        {
            var artistId = -1;
            string sproc = "adm_ArtistInsert";

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FirstName", artist.FirstName);
                parameters.Add("@LastName", artist.LastName);
                parameters.Add("@Biography", artist.Biography);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                artistId = await _db.SaveDataReturnId(sproc, parameters);
                return artistId;
            }
            catch (Exception)
            {
                return artistId;
            }
        }

        public async Task<int> UpdateArtistAsync(Artist artist)
        {
            var artistId = -1;

            try
            {
                string sproc = "up_UpdateArtist";
                var parameters = new DynamicParameters();
                parameters.Add("@ArtistId", artist.ArtistId);
                parameters.Add("@FirstName", artist.FirstName);
                parameters.Add("@LastName", artist.LastName);
                parameters.Add("@Name", artist.Name);
                parameters.Add("@Biography", artist.Biography);
                parameters.Add("@Result", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                artistId = await _db.SaveDataReturnId(sproc, parameters);
                return artistId;
            }
            catch (Exception)
            {
                return artistId;
            }
        }

        public async Task DeleteAsync(int artistId)
        {
            try
            {
                string sproc = "up_deleteArtist";
                var parameter = new DynamicParameters();
                parameter.Add("@ArtistId", artistId);
                await _db.SaveData(sproc, parameter);
            }
            catch (Exception)
            {
                // swallow — caller receives 204 regardless
            }
        }

        public async Task<Artist?> GetArtistByRecordIdAsync(int recordId)
        {
            var sproc = "up_ArtistSelectByRecordId";
            var parameter = new DynamicParameters();
            parameter.Add("@RecordId", recordId);
            return await _db.GetFirstOrDefault<Artist, dynamic>(sproc, parameter);
        }

        public async Task<string> GetBiographyAsync(int recordId)
        {
            var sproc = "up_getBiography";
            var parameter = new DynamicParameters();
            parameter.Add("@RecordId", recordId);
            return await _db.GetScalar<string, dynamic>(sproc, parameter) ?? string.Empty;
        }

        public async Task<int> GetArtistIdAsync(string firstName, string lastName)
        {
            var sproc = "up_getArtistID";
            var parameters = new { FirstName = firstName, LastName = lastName };
            int? artistId = await _db.GetScalar<int, object>(sproc, parameters);
            return artistId ?? 0;
        }

        public async Task<int> GetArtistIdAsync(int recordId)
        {
            string sproc = "up_getArtistIdFromRecord";
            var parameters = new DynamicParameters();
            parameters.Add("@RecordId", recordId);
            parameters.Add("@ArtistId", dbType: DbType.Int32, direction: ParameterDirection.Output);
            return await _db.SaveDataReturnId(sproc, parameters, "@ArtistId");
        }
    }
}
