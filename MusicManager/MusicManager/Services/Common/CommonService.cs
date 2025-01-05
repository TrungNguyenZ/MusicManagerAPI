using MusicManager.Models;
using MusicManager.Repositories;
using System.Globalization;

namespace MusicManager.Services
{
    public class CommonService : ICommonService
    {
        private readonly IRepository<ApplicationUser> _repositoryUser;
        public CommonService(IRepository<ApplicationUser> repositoryUser)
        {
            _repositoryUser = repositoryUser;
        }
        public decimal ConvertDecimal (String input)
        {
            input = input.Replace(",", ".");
            decimal decimalValue = decimal.Parse(input, NumberStyles.Float, CultureInfo.InvariantCulture);
            return decimalValue;
        }
        public long GetNetSinger(double revenuePercentage, long value)
        {
            var gross = value * revenuePercentage / 100;
            var net = (long)(gross * 90 / 100) * 90 / 100;
            return net;
        }
        public long GetNetSinger(double revenuePercentage, double value)
        {
            var gross = value * revenuePercentage / 100;
            var net = (long)(gross * 90 / 100) * 90 / 100;
            return net;
        }   
        public long GetNetSinger(string revenuePercentage, double value)
        {
            var gross = value * Double.Parse(revenuePercentage) / 100;
            var net = (long)(gross * 90 / 100) * 90 / 100;
            return net;
        } 
        public long GetNetSinger(string revenuePercentage, long value)
        {
            var gross = value * Double.Parse(revenuePercentage) / 100;
            var net = (long)(gross * 90 / 100) * 90 / 100;
            return net;
        }
    }
}
