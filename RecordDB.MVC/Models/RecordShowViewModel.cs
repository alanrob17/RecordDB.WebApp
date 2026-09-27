using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Models
{
    /// <summary>
    /// View model for the RecordView show page.
    /// Combines the record projection (from up_RecordSelectByIdCore)
    /// with associated tracks (from up_GetArtistRecordTracks)
    /// and ensures the artist biography is available.
    /// </summary>
    public class RecordShowViewModel
    {
        public ArtistRecordDto Record { get; set; } = new();

        public IEnumerable<ArtistRecordDiscTrackDto> Tracks { get; set; } = [];

        /// <summary>Returns true if the record has one or more associated tracks.</summary>
        public bool HasTracks => Tracks.Any(t => t.TrackId.HasValue && t.TrackId.Value > 0);

        /// <summary>Total track count.</summary>
        public int TrackCount => Tracks.Count(t => t.TrackId.HasValue && t.TrackId.Value > 0);

        /// <summary>Total playtime across all tracks formatted as mm:ss or hh:mm:ss.</summary>
        public string TotalPlayTime
        {
            get
            {
                var totalSeconds = Tracks
                    .Where(t => t.TrackLength.HasValue && t.TrackLength.Value > 0)
                    .Sum(t => t.TrackLength!.Value);

                if (totalSeconds <= 0) return string.Empty;

                var ts = TimeSpan.FromSeconds(totalSeconds);
                return ts.TotalHours >= 1
                    ? $"{(int)ts.TotalHours}h {ts.Minutes:D2}m {ts.Seconds:D2}s"
                    : $"{ts.Minutes}m {ts.Seconds:D2}s";
            }
        }
    }
}
