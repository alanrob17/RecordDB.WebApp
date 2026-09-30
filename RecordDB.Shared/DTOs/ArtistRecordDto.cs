namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Flat projection joining Artist + Record fields, returned by several stored procedures.
    /// </summary>
    public class ArtistRecordDto
    {
        public int      ArtistId   { get; set; }
        public string?  FirstName  { get; set; }
        public string?  LastName   { get; set; }
        public string?  ArtistName { get; set; }
        public string?  Biography  { get; set; }

        public int      RecordId   { get; set; }
        public string?  Name       { get; set; }
        public string?  Field      { get; set; }
        public int      Recorded   { get; set; }
        public string?  Label      { get; set; }
        public string?  Pressing   { get; set; }
        public string?  Rating     { get; set; }
        public int      Discs      { get; set; }
        public string?  Media      { get; set; }
        public DateTime? Bought    { get; set; }
        public decimal? Cost       { get; set; }
        public string?  CoverName  { get; set; }
        public string?  Review     { get; set; }

        public ArtistDto Artist => new() { ArtistId = ArtistId, Name = ArtistName ?? $"{FirstName} {LastName}".Trim() };
    }
}
