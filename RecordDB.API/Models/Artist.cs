namespace RecordDB.API.Models
{
    /// <summary>
    /// Represents an Artist as stored in the database.
    /// Navigation properties are intentionally omitted — this project uses Dapper, not EF Core.
    /// </summary>
    public class Artist
    {
        public int     ArtistId  { get; set; }
        public string? FirstName { get; set; }
        public string? LastName  { get; set; }
        public string? Name      { get; set; }
        public string? Biography { get; set; }

        public override string ToString()
        {
            var biography = string.IsNullOrEmpty(Biography)
                ? "No Biography"
                : (Biography.Length > 30 ? Biography[..30] + "..." : Biography);

            return $"Artist Id: {ArtistId}, Artist: {Name}, Biography: {biography}";
        }
    }
}
