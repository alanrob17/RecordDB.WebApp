using System.ComponentModel.DataAnnotations;

namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Request body DTO for creating a new Disc (POST /api/disc).
    /// </summary>
    public class CreateDiscDto
    {
        [Required]
        public int     RecordId     { get; set; }

        [Required]
        public int     DiscNo       { get; set; }

        public int?    FreeDbDiscId { get; set; }
        public string? FreeDbId     { get; set; }
        public int?    Length       { get; set; }
    }
}
