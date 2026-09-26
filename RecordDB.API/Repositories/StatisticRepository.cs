using RecordDB.API.Data;
using RecordDB.API.Models;
using System.Data;

namespace RecordDB.API.Repositories
{
    public class StatisticRepository(IDataAccess db) : IStatisticRepository
    {
        private readonly IDataAccess _db = db;

        #region " Methods "

        public async Task<Statistic> GetStatisticsAsync()
        {
            var statistics = new Statistic();

            // query for total number of CD's
            var squery = "select sum(discs) from record where media = 'CD'";
            statistics.TotalCDs = await GetCountAsync(squery);

            // query for total number of records
            squery = "select sum(discs) from record where media='R'";
            statistics.TotalRecords = await GetCountAsync(squery);

            // query for number of Rock discs
            squery = "select sum(discs) from record where field = 'Rock'";
            statistics.RockDisks = await GetCountAsync(squery);

            // query for number of Folk discs
            squery = "select sum(discs) from record where field = 'Folk'";
            statistics.FolkDisks = await GetCountAsync(squery);

            // query for number of Acoustic discs
            squery = "select sum(discs) from record where field = 'Acoustic'";
            statistics.AcousticDisks = await GetCountAsync(squery);

            // query for number of Jazz and Fusion discs
            squery = "select sum(discs) from record where field = 'Jazz' or field = 'Fusion'";
            statistics.JazzDisks = await GetCountAsync(squery);

            // query for number of Blues discs
            squery = "select sum(discs) from record where field = 'Blues'";
            statistics.BluesDisks = await GetCountAsync(squery);

            // query for number of Country discs
            squery = "select sum(discs) from record where field = 'Country'";
            statistics.CountryDisks = await GetCountAsync(squery);

            // query for number of Classical discs
            squery = "select sum(discs) from record where field = 'Classical'";
            statistics.ClassicalDisks = await GetCountAsync(squery);

            // query for number of Soundtrack discs
            squery = "select sum(discs) from record where field = 'Soundtrack'";
            statistics.SoundtrackDisks = await GetCountAsync(squery);

            // query for number of Four Star records
            squery = "select count(*) from record where Rating = '****'";
            statistics.FourStarDisks = await GetCountAsync(squery);

            // query for number of Three Star records
            squery = "select count(*) from record where Rating = '***'";
            statistics.ThreeStarDisks = await GetCountAsync(squery);

            // query for number of Two Star records
            squery = "select count(*) from record where Rating = '**'";
            statistics.TwoStarDisks = await GetCountAsync(squery);

            // query for number of One Star records
            squery = "select count(*) from record where Rating = '*'";
            statistics.OneStarDisks = await GetCountAsync(squery);

            // query for amount spent on records (vinyl)
            squery = "select sum(cost) from record where media = 'R'";
            statistics.RecordCost = await GetCostAsync(squery);

            // query for amount spent on CD's
            squery = "select sum(cost) from record where media = 'CD'";
            statistics.CDCost = await GetCostAsync(squery);

            // calculate the average cost of all CDs
            statistics.AvCDCost = statistics.TotalCDs > 0
                ? statistics.CDCost / (decimal)statistics.TotalCDs
                : 0.00m;

            // query for total amount spent on all media
            squery = "select sum(cost) from record";
            statistics.TotalCost = await GetCostAsync(squery);

            // 2017 ─────────────────────────────────────────────────────────────
            squery = "select sum(discs) from record where bought > '31-Dec-2016' and bought < '01-Jan-2018'";
            statistics.Disks2017 = await GetCountAsync(squery);

            squery = "select sum(cost) from record where bought > '31-Dec-2016' and bought < '01-Jan-2018'";
            statistics.Cost2017 = await GetCostAsync(squery);
            statistics.Av2017 = CalculateAverage(statistics.Cost2017, statistics.Disks2017);

            // 2018 ─────────────────────────────────────────────────────────────
            squery = "select sum(discs) from record where bought > '31-Dec-2017' and bought < '01-Jan-2019'";
            statistics.Disks2018 = await GetCountAsync(squery);

            squery = "select sum(cost) from record where bought > '31-Dec-2017' and bought < '01-Jan-2019'";
            statistics.Cost2018 = await GetCostAsync(squery);
            statistics.Av2018 = CalculateAverage(statistics.Cost2018, statistics.Disks2018);

            // 2019 ─────────────────────────────────────────────────────────────
            squery = "select sum(discs) from record where bought > '31-Dec-2018' and bought < '01-Jan-2020'";
            statistics.Disks2019 = await GetCountAsync(squery);

            squery = "select sum(cost) from record where bought > '31-Dec-2018' and bought < '01-Jan-2020'";
            statistics.Cost2019 = await GetCostAsync(squery);
            statistics.Av2019 = CalculateAverage(statistics.Cost2019, statistics.Disks2019);

            // 2020 ─────────────────────────────────────────────────────────────
            squery = "select sum(discs) from record where bought > '31-Dec-2019' and bought < '01-Jan-2021'";
            statistics.Disks2020 = await GetCountAsync(squery);

            squery = "select sum(cost) from record where bought > '31-Dec-2019' and bought < '01-Jan-2021'";
            statistics.Cost2020 = await GetCostAsync(squery);
            statistics.Av2020 = CalculateAverage(statistics.Cost2020, statistics.Disks2020);

            // 2021 ─────────────────────────────────────────────────────────────
            squery = "select sum(discs) from record where bought > '31-Dec-2020' and bought < '01-Jan-2022'";
            statistics.Disks2021 = await GetCountAsync(squery);

            squery = "select sum(cost) from record where bought > '31-Dec-2020' and bought < '01-Jan-2022'";
            statistics.Cost2021 = await GetCostAsync(squery);
            statistics.Av2021 = CalculateAverage(statistics.Cost2021, statistics.Disks2021);

            // 2022 ─────────────────────────────────────────────────────────────
            squery = "select sum(discs) from record where bought > '31-Dec-2021' and bought < '01-Jan-2023'";
            statistics.Disks2022 = await GetCountAsync(squery);

            squery = "select sum(cost) from record where bought > '31-Dec-2021' and bought < '01-Jan-2023'";
            statistics.Cost2022 = await GetCostAsync(squery);
            statistics.Av2022 = CalculateAverage(statistics.Cost2022, statistics.Disks2022);

            return statistics;
        }

        #endregion

        #region " Private Helpers "

        /// <summary>
        /// Executes an inline SQL query that returns an integer count or sum (e.g. COUNT, SUM of discs).
        /// Returns 0 if the result is NULL.
        /// </summary>
        private async Task<int> GetCountAsync(string sql)
        {
            var result = await _db.GetScalar<int?, object>(sql, new { }, CommandType.Text);
            return result ?? 0;
        }

        /// <summary>
        /// Executes an inline SQL query that returns a decimal cost sum (e.g. SUM of cost).
        /// Returns 0.00 if the result is NULL.
        /// </summary>
        private async Task<decimal> GetCostAsync(string sql)
        {
            var result = await _db.GetScalar<decimal?, object>(sql, new { }, CommandType.Text);
            return result ?? 0.00m;
        }

        /// <summary>
        /// Calculates the average cost per disc, guarding against divide-by-zero
        /// and treating a zero total cost as "nothing bought".
        /// </summary>
        private static decimal CalculateAverage(decimal cost, int discs)
        {
            if (cost <= 0 || discs == 0)
                return 0.00m;

            return cost / (decimal)discs;
        }

        #endregion
    }
}
