using System.ComponentModel.DataAnnotations;

namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Request body DTO for updating an existing Artist (PUT /api/artist/{id}).
    /// </summary>
    public class UpdateArtistDto
    {
        [Required]
        public int ArtistId { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }

        public string? Biography { get; set; }
    }
}
