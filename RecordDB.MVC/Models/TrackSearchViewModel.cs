namespace RecordDB.MVC.Models
{
    /// <summary>
    /// View model for the Track Search form.
    /// Captures the partial or full track name entered by the user.
    /// </summary>
    public class TrackSearchViewModel
    {
        /// <summary>Partial or full track name to search for.</summary>
        public string? TrackName { get; set; }
    }
}
