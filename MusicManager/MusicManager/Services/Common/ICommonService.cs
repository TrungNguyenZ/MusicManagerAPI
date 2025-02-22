using MusicManager.Models;

namespace MusicManager.Services
{
    public interface ICommonService
    {
        decimal ConvertDecimal(String input);
        long GetNetSinger(object revenuePercentage, object value);
        long GetNetEnterprise(object value);
        Task<string> GetAccessTokenAsync();
        Task SendNotificationToTopicAsync(string accessToken, string title, string body, string topic);
        Task SendEmailAsync(List<string> toList, string subject, string body);
        Task SendEmaiNoticationlAsync(string subject, string body);
    }
}
 