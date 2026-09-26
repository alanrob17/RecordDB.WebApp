using RecordDB.Shared.DTOs;
using RecordDB.API.Models;

namespace RecordDB.API.Repositories
{
    public interface IRecordRepository
    {
        /// <summary>Returns a single record with artist details by record ID.</summary>
        Task<ArtistRecordDto?> SelectAsync(int recordId);

        /// <summary>Returns all records with artist details.</summary>
        Task<List<ArtistRecordDto>> SelectAsync();

        /// <summary>Returns records filtered by the show flag (e.g. "Y"/"N").</summary>
        Task<List<Record>> SelectByShowAsync(string show);

        /// <summary>Returns all records with artist details filtered by the show flag.</summary>
        Task<List<ArtistRecordDto>> SelectRecordsShowAsync(string show);

        /// <summary>Returns all records (with Artist navigation) for a given artist.</summary>
        Task<List<Record>> GetArtistRecordsAsync(int artistId);

        /// <summary>Returns a flat record list for a given artist (used for dropdowns).</summary>
        Task<List<Record>> SelectArtistRecordsAsync(int artistId);

        /// <summary>Returns all records that have a review, as a slim DTO.</summary>
        Task<List<RecordReviewDto>> SelectRecordReviewsAsync();

        /// <summary>Returns all records by artist name.</summary>
        Task<List<ArtistRecordDto>> GetRecordsByArtistNameAsync(string name);

        /// <summary>Returns all records by recording year.</summary>
        Task<List<ArtistRecordDto>> GetRecordsByYearAsync(int recorded);

        /// <summary>Returns per-artist totals (disc count + cost).</summary>
        Task<List<Total>> GetTotalCostsAsync();

        /// <summary>Returns all records that have no tracks.</summary>
        Task<List<ArtistRecordDiscDto>> ListRecordsWithNoTracksAsync();

        /// <summary>Returns records with no tracks for a given artist name.</summary>
        Task<List<ArtistRecordDiscDto>> ArtistRecordsWithNoTracksAsync(string name);

        /// <summary>Returns tracks (with record/disc context) where the track name contains <paramref name="name"/>.</summary>
        Task<List<ArtistRecordDiscTrackDto>> SelectTracksByPartialNameAsync(string name);

        /// <summary>Returns records that are missing a review.</summary>
        Task<List<MissingReviewDto>> NoRecordReviewsAsync();

        /// <summary>Returns the total disc count for the given show flag as a formatted string.</summary>
        Task<string> CountDiscsAsync(string show);

        /// <summary>Returns the number of records for a given artist as a formatted string.</summary>
        Task<string> GetArtistNumberOfRecordsAsync(int artistId);

        /// <summary>Returns the number of records for a given recording year as a formatted string.</summary>
        Task<string> GetRecordedYearNumberAsync(int year);

        /// <summary>Creates a new record and returns the new RecordId, or -1 on failure.</summary>
        Task<int> InsertAsync(Record record);

        /// <summary>Updates an existing record using the ArtistRecordDto; returns the RecordId.</summary>
        Task<int> UpdateAsync(ArtistRecordDto record);

        /// <summary>Deletes a record by primary key.</summary>
        Task DeleteAsync(int recordId);
    }
}
