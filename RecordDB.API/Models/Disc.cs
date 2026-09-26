namespace RecordDB.API.Models
{
    /// <summary>
    /// Represents a Disc within a Record.
    /// Navigation properties are intentionally omitted — this project uses Dapper, not EF Core.
    /// </summary>
    public class Disc
    {
        public int     DiscId       { get; set; }
        public int     RecordId     { get; set; }
        public int     DiscNo       { get; set; }
        public int?    FreeDbDiscId { get; set; }
        public string? FreeDbId     { get; set; }
        public int?    Length       { get; set; }
    }
}
