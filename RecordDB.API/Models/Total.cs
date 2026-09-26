namespace RecordDB.API.Models
{
    /// <summary>
    /// Per-artist total disc count and cost, returned by sp_getTotalsForEachArtist.
    /// </summary>
    public class Total
    {
        public int     ArtistId   { get; set; }
        public string? Name       { get; set; }
        public int     TotalDiscs { get; set; }
        public decimal TotalCost  { get; set; }
    }
}
