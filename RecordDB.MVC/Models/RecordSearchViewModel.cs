using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Models
{
    /// <summary>
    /// View model for Record Search by partial record name.
    /// Supports filtering the complete list (up_RecordSelectAll)
    /// and selecting an individual record (up_RecordSelectByIdCore).
    /// </summary>
    public class RecordSearchViewModel
    {
        /// <summary>Partial record name entered by the user.</summary>
        public string? RecordName { get; set; }

        /// <summary>Filtered records matching the partial record name.</summary>
        public IEnumerable<ArtistRecordDto>? Results { get; set; }

        /// <summary>The ID of the record selected by the user from the filtered list.</summary>
        public int? SelectedRecordId { get; set; }

        /// <summary>The full record details retrieved via SelectAsync(selectedRecordId) / up_RecordSelectByIdCore.</summary>
        public ArtistRecordDto? SelectedRecord { get; set; }

        /// <summary>True once a search has been executed.</summary>
        public bool Searched { get; set; }

        public bool HasResults => Results?.Any() == true;
        public int ResultCount => Results?.Count() ?? 0;
    }
}
