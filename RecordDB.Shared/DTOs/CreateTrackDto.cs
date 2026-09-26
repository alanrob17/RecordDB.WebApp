using System.ComponentModel.DataAnnotations;

namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Request body DTO for creating a new Track (POST /api/track).
    /// </summary>
    public class CreateTrackDto
    {
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
