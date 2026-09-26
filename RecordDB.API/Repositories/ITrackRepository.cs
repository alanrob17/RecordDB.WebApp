using RecordDB.Shared.DTOs;
using RecordDB.API.Models;

namespace RecordDB.API.Repositories
{
    public interface ITrackRepository
    {
        /// <summary>Returns all tracks with artist, record, and disc context.</summary>
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectAllTrackEntitiesAsync();

        /// <summary>Returns tracks for records by artist name.</summary>
        /// <param name="name">Artist name to search for.</param>
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectArtistRecordTracksAsync(string name);

        /// <summary>Returns tracks for records matching the given record name.</summary>
        /// <param name="name">Record name to search for.</param>
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectTracksByRecordAsync(string name);

        /// <summary>Returns tracks matching a partial track name.</summary>
        /// <param name="name">Partial track name to search for.</param>
        Task<IEnumerable<ArtistRecordDiscTrackDto>> SelectTracksByPartialNameAsync(string name);

        /// <summary>Returns a single track by track ID, or null.</summary>
        /// <param name="trackId">The primary key of the track.</param>
        Task<ArtistRecordDiscTrackDto?> SelectTrackByIdAsync(int trackId);

        /// <summary>Returns the total number of tracks for a given record ID.</summary>
        /// <param name="recordId">The record's primary key.</param>
        Task<int> GetTrackNumberAsync(int recordId);

        /// <summary>Creates a new track and returns the new TrackId, or -1 on failure.</summary>
        /// <param name="track">The track entity to insert.</param>
        Task<int> InsertTrackAsync(Track track);

        /// <summary>Updates an existing track and returns the TrackId, or -1 on failure.</summary>
        /// <param name="track">The track entity with updated values.</param>
        Task<int> UpdateTrackAsync(Track track);

        /// <summary>Deletes a track by primary key.</summary>
        /// <param name="trackId">The track's primary key.</param>
        Task DeleteTrackAsync(int trackId);

        /// <summary>
        /// Bulk-inserts a list of tracks using the TrackTableType TVP.
        /// </summary>
        /// <param name="tracks">The list of tracks to insert.</param>
        Task BulkTrackInsertAsync(List<Track> tracks);

        /// <summary>
        /// Checks whether the specified disc already has tracks.
        /// Returns the number of existing tracks.
        /// </summary>
        /// <param name="discId">The disc ID to check.</param>
        Task<int> CheckForTracksAsync(int discId);

        /// <summary>
        /// Bulk-inserts a collection of tracks using the TrackTableType TVP.
        /// </summary>
        /// <param name="tracks">The collection of tracks to insert.</param>
        Task BulkInsertTracksAsync(IEnumerable<Track> tracks);
    }
}
