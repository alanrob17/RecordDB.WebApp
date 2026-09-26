namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Slim projection for records that have a review, used by SelectRecordReviewsAsync.
    /// </summary>
    public class RecordReviewDto
    {
        public string? Name   { get; set; }
        public string? Title  { get; set; }
        public string  Review { get; set; } = string.Empty;
    }
}
