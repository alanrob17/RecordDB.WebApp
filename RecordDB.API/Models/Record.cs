namespace RecordDB.API.Models
{
    /// <summary>
    /// Represents a Record (album) as stored in the database.
    /// Navigation properties are intentionally omitted — this project uses Dapper, not EF Core.
    /// </summary>
    public class Record
    {
        public int      RecordId  { get; set; }
        public int      ArtistId  { get; set; }
        public string?  Name      { get; set; }
        public string?  Field     { get; set; }
        public int      Recorded  { get; set; }
        public string?  Label     { get; set; }
        public string?  Pressing  { get; set; }
        public string?  Rating    { get; set; }
        public int      Discs     { get; set; }
        public string?  Media     { get; set; }
        public DateTime? Bought   { get; set; }
        public decimal?  Cost     { get; set; }
        public string?  CoverName { get; set; }
        public string?  Review    { get; set; }

        public override string ToString() =>
            $"Record Id: {RecordId}, Name: {Name}, Recorded: {Recorded}, Media: {Media}";
    }
}
