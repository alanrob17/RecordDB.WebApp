namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Slim projection for records that have no review text.
    /// </summary>
    public class MissingReviewDto
    {
        public int     RecordId { get; set; }
        public string? Name     { get; set; }
        public string? Record   { get; set; }
    }
}
