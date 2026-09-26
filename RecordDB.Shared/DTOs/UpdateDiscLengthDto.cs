using System.ComponentModel.DataAnnotations;

namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Request body DTO for patching disc length only (PATCH /api/disc/{id}/length).
    /// </summary>
    public class UpdateDiscLengthDto
    {
        public int? Length { get; set; }
    }
}
