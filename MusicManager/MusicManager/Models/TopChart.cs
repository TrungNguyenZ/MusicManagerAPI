namespace MusicManager.Models
{
    public class TopChartArtist
    {
        public string artistName { get; set; }
        public long sum { get; set; }
        public decimal percentage { get; set; }
    }
    public class TopChartTrack
    {
        public string catalogueTitle { get; set; }
        public string artistName { get; set; }
        public long sum { get; set; }
        public decimal percentage { get; set; }
    }
}
