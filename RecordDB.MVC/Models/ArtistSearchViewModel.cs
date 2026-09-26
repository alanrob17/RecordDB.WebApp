namespace RecordDB.MVC.Models
{
    /// <summary>
    /// Simple view model that carries the artist search form fields
    /// and any matching results back to the Search view.
    /// </summary>
    public class ArtistSearchViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName  { get; set; }

        /// <summary>Artists returned by the API search (null until a search is performed).</summary>
        public IEnumerable<RecordDB.Shared.DTOs.ArtistDto>? Results { get; set; }

        /// <summary>True once the form has been submitted at least once.</summary>
        public bool Searched { get; set; }

        public bool HasResults  => Results?.Any() == true;
        public int  ResultCount => Results?.Count() ?? 0;
    }
}
