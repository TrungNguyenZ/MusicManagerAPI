namespace MusicManager.Models
{
    public class DigitalMonthSumModel
    {
        public int month { get; set; } = 0;
        public int year { get; set; } = 0;
        public string digitalServiceProvider { get; set; }
        public long sum { get; set; } = 0;
    }
    public class DigitalQuarterSumModel
    {
        public int quarter { get; set; } = 0;
        public int year { get; set; } = 0;
        public string digitalServiceProvider { get; set; }
        public long sum { get; set; } = 0;
    }
    public class DigitalYearSumModel
    {
        public int quarterYear { get; set; } = 0;
        public string digitalServiceProvider { get; set; }
        public long sum { get; set; } = 0;
    }
    public class DigitalQuarterPercentModel
    {
        public int quarter { get; set; } = 0;
        public int quarterYear { get; set; } = 0;
        public string digitalServiceProvider { get; set; }
        public long sum { get; set; } = 0;
    }
    public class DigitalYearPercentModel
    {
        public int quarter { get; set; } = 0;
        public int quarterYear { get; set; } = 0;
        public string digitalServiceProvider { get; set; } 
        public long sum { get; set; } = 0;
    }
    public class CountryPercentModel
    {
        public int quarter { get; set; } = 0;
        public int quarterYear { get; set; } = 0;
        public string countryCode { get; set; } = "";
        public long sum { get; set; } = 0;
    }
    public class StatisticModel
    {

        public int month { get; set; } = 0;
        public int year { get; set; } = 0;
        public int quarter { get; set; } = 0;
        public int quarterYear { get; set; } = 0;
        public string countryCode { get; set; } = "";
        public long sum { get; set; } = 0;
    }
}
