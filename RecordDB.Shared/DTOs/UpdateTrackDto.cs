using System.ComponentModel.DataAnnotations;

namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Request body DTO for updating an existing Track (PUT /api/track/{id}).
    /// </summary>
    public class UpdateTrackDto
    {
        [Required]
        public int TrackId { get; set; }

        [Required]
        public int DiscId { get; set; }

        [Required]
        public int TrackNo { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        public int? TrackLength { get; set; }

        [MaxLength(200)]
        public string? Extended { get; set; }
    }
}
