namespace RecordDB.Shared.DTOs
{
    /// <summary>
    /// Projection joining Artist + Record + Disc + Track fields, used for track-search results.
    /// </summary>
    public class ArtistRecordDiscTrackDto
    {
        public int     RecordId     { get; set; }
        public int     DiscId       { get; set; }
        public string? ArtistName   { get; set; }
        public string? Name         { get; set; }
        public int     DiscNo       { get; set; }
        public int?    FreeDbDiscId { get; set; }
        public string? FreeDbId     { get; set; }
        public int?    Length       { get; set; }
        public int?    TrackId      { get; set; }
        public int?    TrackNo      { get; set; }
        public string? TrackName    { get; set; }
        public int?    TrackLength  { get; set; }
        public string? Extended     { get; set; }
    }
}
