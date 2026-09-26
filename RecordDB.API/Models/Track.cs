namespace RecordDB.API.Models
{
    /// <summary>
    /// Represents a Track on a Disc as stored in the database.
    /// Navigation properties are intentionally omitted — this project uses Dapper, not EF Core.
    /// </summary>
    public class Track
    {
        public int     TrackId     { get; set; }
        public int     DiscId      { get; set; }
        public int     TrackNo     { get; set; }
        public string? Name        { get; set; }
        public int?    TrackLength { get; set; }
        public string? Extended    { get; set; }

        public override string ToString() =>
            $"Track Id: {TrackId}, Disc Id: {DiscId}, Track No: {TrackNo}, Name: {Name}";
    }
}
