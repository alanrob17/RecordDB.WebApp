using System.Globalization;

namespace RecordDB.API.Extensions
{
    /// <summary>
    /// DateTime formatting helpers used by RecordRepository.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>Returns a short date string (e.g. "15-Jan-2024") or "unk" if <paramref name="bought"/> is null.</summary>
        public static string ToShortDate(object? bought)
        {
            if (bought is DateTime dt)
                return dt.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);

            return "unk";
        }
    }
}
