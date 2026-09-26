using RecordDB.API.Models;

namespace RecordDB.API.Repositories
{
    public interface IArtistRepository
    {
        /// <summary>Returns all artists with full details.</summary>
        Task<IEnumerable<Artist>> GetArtistsAsync();

        /// <summary>Returns all artists whose name contains <paramref name="name"/>.</summary>
        Task<IEnumerable<Artist>> GetArtistsByPartialNameAsync(string name);

        /// <summary>Returns a flat list of all artists (minimal stored procedure).</summary>
        Task<List<Artist>> SelectAsync();

        /// <summary>Returns a single artist by primary key, or null.</summary>
        Task<Artist?> SelectAsync(int artistId);

        /// <summary>Returns all artists that have no biography.</summary>
        Task<IEnumerable<Artist>> GetArtistsWithNoBiographyAsync();

        /// <summary>Returns the first artist matching <paramref name="name"/> that has no biography, or null.</summary>
        Task<Artist?> GetArtistWithNoBiographyAsync(string name);

        /// <summary>Creates a new artist and returns the new ArtistId, or -1 on failure.</summary>
        Task<int> InsertAsync(Artist artist);

        /// <summary>Updates an existing artist and returns the ArtistId, or -1 on failure.</summary>
        Task<int> UpdateArtistAsync(Artist artist);

        /// <summary>Deletes an artist by primary key.</summary>
        Task DeleteAsync(int artistId);

        /// <summary>Returns the artist associated with a given record, or null.</summary>
        Task<Artist?> GetArtistByRecordIdAsync(int recordId);

        /// <summary>Returns the biography text for the artist of a given record.</summary>
        Task<string> GetBiographyAsync(int recordId);

        /// <summary>Returns the ArtistId for a given first/last name combination.</summary>
        Task<int> GetArtistIdAsync(string firstName, string lastName);

        /// <summary>Returns the ArtistId for the artist associated with a given record.</summary>
        Task<int> GetArtistIdAsync(int recordId);
    }
}
