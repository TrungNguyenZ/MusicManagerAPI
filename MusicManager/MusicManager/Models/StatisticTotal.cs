namespace MusicManager.Models
{
    public class StatisticTotalModel
    {
        public long TotalForQuarterYear { get; set; } = 0;
        public long TotalForYear { get; set; } = 0;
        public long TotalForAll { get; set; } = 0;
    }
    public class StatisticSumModel
    {
        public List<StatisticDataSumModel> Data { get; set; }
        public List<string> Categories { get; set; }
    }    
    public class StatisticDataSumModel
    {
        public string Name { get; set; }
        public List<long> Data { get; set; }
    }
    public class Statistic
    {
        public string name { get; set; }
        public long sum { get; set; }
    }
    public class StatisticTop
    {
        public Statistic TopCountry { get; set; }
        public Statistic TopDigital { get; set; }
        public int DigitalCount { get; set; }
    }

}
