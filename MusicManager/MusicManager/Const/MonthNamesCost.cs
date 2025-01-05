namespace MusicManager.Const
{
    public static class MonthNames
    {
        private static readonly Dictionary<string, List<string>> MonthsByLanguage = new Dictionary<string, List<string>>
    {
        { "en", new List<string>() { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" } },
        { "vi", new List<string>() { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" } }
    };

        public static List<string> GetMonths(string languageCode)
        {
            // Kiểm tra nếu mã ngôn ngữ tồn tại trong từ điển
            if (MonthsByLanguage.ContainsKey(languageCode))
            {
                return MonthsByLanguage[languageCode];
            }

            // Nếu không tồn tại, mặc định trả về tiếng Anh
            return MonthsByLanguage["vi"];
        }
        private static readonly Dictionary<string, List<string>> QuarterByLanguage = new Dictionary<string, List<string>>
    {
        { "en", new List<string>() { "Q1", "Q2", "Q3", "Q4" } },
        { "vi", new List<string>() { "Quý 1", "Quý 2", "Quý 3", "Quý 4", } }
    };

        public static List<string> GetQuarter(string languageCode)
        {
            // Kiểm tra nếu mã ngôn ngữ tồn tại trong từ điển
            if (QuarterByLanguage.ContainsKey(languageCode))
            {
                return QuarterByLanguage[languageCode];
            }

            // Nếu không tồn tại, mặc định trả về tiếng Anh
            return QuarterByLanguage["vi"];
        }
    }
}
