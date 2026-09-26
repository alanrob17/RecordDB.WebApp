namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Response DTO returned to API consumers for Artist endpoints.
    /// </summary>
    public class ArtistDto
    {
        public int     ArtistId  { get; set; }
        public string? FirstName { get; set; }
        public string? LastName  { get; set; }
        public string? Name      { get; set; }
        public string? Biography { get; set; }
    }
}
