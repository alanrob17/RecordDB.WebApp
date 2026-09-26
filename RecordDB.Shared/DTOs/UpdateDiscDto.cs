using System.ComponentModel.DataAnnotations;

namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Request body DTO for updating an existing Disc (PUT /api/disc/{id}).
    /// </summary>
    public class UpdateDiscDto
    {
        [Required]
        public int     DiscId       { get; set; }

        [Required]
        public int     DiscNo       { get; set; }

        public int?    FreeDbDiscId { get; set; }
        public string? FreeDbId     { get; set; }
        public int?    Length       { get; set; }
    }
}
