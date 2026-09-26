using RecordDB.Shared.DTOs;
using RecordDB.API.Models;

namespace RecordDB.API.Repositories
{
    public interface IDiscRepository
    {
        /// <summary>Returns all discs with artist and record context.</summary>
        Task<IEnumerable<ArtistRecordDiscDto>> SelectAllDiscEntitiesAsync();

        /// <summary>Returns all discs for records whose name contains <paramref name="recordName"/>.</summary>
        Task<IEnumerable<ArtistRecordDiscDto>> GetDiscRecordsByRecordNameAsync(string recordName);

        /// <summary>Returns a single disc with artist and record context by disc ID, or null.</summary>
        Task<ArtistRecordDiscDto?> SelectSingleDiscAsync(int discId);

        /// <summary>Creates a new disc and returns the new DiscId, or -1 on failure.</summary>
        Task<int> InsertDiscAsync(Disc disc);

        /// <summary>Updates an existing disc and returns the DiscId, or -1 on failure.</summary>
        Task<int> UpdateDiscAsync(Disc disc);

        /// <summary>Deletes a disc by primary key.</summary>
        Task DeleteDiscAsync(int discId);

        /// <summary>Updates only the Length field of a disc.</summary>
        Task UpdateDiscLengthAsync(int discId, int? totalLength);
    }
}
