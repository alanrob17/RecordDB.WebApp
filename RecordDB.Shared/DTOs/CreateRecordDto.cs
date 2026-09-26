using System.ComponentModel.DataAnnotations;

namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Request body DTO for creating a new Record (POST /api/record).
    /// </summary>
    public class CreateRecordDto
    {
        [Required]
        public int ArtistId { get; set; }

        [Required, MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Field { get; set; }

        public int Recorded { get; set; }

        [MaxLength(50)]
        public string? Label { get; set; }

        [MaxLength(50)]
        public string? Pressing { get; set; }

        [MaxLength(4)]
        public string? Rating { get; set; }

        public int Discs { get; set; }

        [MaxLength(50)]
        public string? Media { get; set; }

        public DateTime? Bought { get; set; }

        public decimal? Cost { get; set; }

        [MaxLength(50)]
        public string? CoverName { get; set; }

        public string? Review { get; set; }
    }
}
