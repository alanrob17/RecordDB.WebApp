using System.ComponentModel.DataAnnotations;

namespace RecordDB.API.Models
{
    public class Statistic
    {
        #region " Properties "

        public int TotalCDs { get; set; }

        public int RockDisks { get; set; }

        public int FolkDisks { get; set; }

        public int AcousticDisks { get; set; }

        public int JazzDisks { get; set; }

        public int BluesDisks { get; set; }

        public int CountryDisks { get; set; }

        public int ClassicalDisks { get; set; }

        public int SoundtrackDisks { get; set; }

        public int FourStarDisks { get; set; }

        public int ThreeStarDisks { get; set; }

        public int TwoStarDisks { get; set; }

        public int OneStarDisks { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal RecordCost { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal CDCost { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AvCDCost { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalCost { get; set; }

        public int Disks2017 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Cost2017 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Av2017 { get; set; }

        public int Disks2018 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Cost2018 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Av2018 { get; set; }

        public int Disks2019 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Cost2019 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Av2019 { get; set; }

        public int Disks2020 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Cost2020 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Av2020 { get; set; }

        public int Disks2021 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Cost2021 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Av2021 { get; set; }

        public int Disks2022 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Cost2022 { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Av2022 { get; set; }

        public int TotalRecords { get; set; }

        #endregion
    }
}
