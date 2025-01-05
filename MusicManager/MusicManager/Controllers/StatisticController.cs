using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicManager.Const;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Services;
using OfficeOpenXml;
using System.Globalization;

namespace MusicManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        private readonly ICommonService _commonService;
        public StatisticController(IStatisticService statisticService, ICommonService commonService)
        {
            _statisticService = statisticService;
            _commonService = commonService;
        }
        [Authorize]
        [HttpPost("total")]
        public async Task<IActionResult> Total(int quarter, int quarterYear)
        {
            try
            {
                var res = new ResponseData<StatisticTotalModel>();
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var revenuePercentage = User.FindFirst("revenuePercentage").Value;
                if (isAdminClaim == "True")
                {
                    res.data = await _statisticService.GetTotal(quarter, quarterYear);
                }
                else
                {
                    res.data = await _statisticService.GetTotalBySinger(quarter, quarterYear, artistName, Double.Parse(revenuePercentage));
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                var bad = new ResponseBase()
                {
                    isSuccess = false,
                    message = ex.Message,
                    code = 500
                };
                return Ok(bad);
            }
        }
        [HttpPost("digital-total")]
        public async Task<IActionResult> digitalSum(int type, int year, string? language)
        {
            try
            {
                if (type == 1)
                {
                    var data = await DigitalSumMonth(year, language);
                    return Ok(data);
                }
                else if (type == 2)
                {
                    var data = await DigitalSumQuarter(year, language);
                    return Ok(data);
                }
                else
                {
                    var data = await DigitalSumYear();
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                var bad = new ResponseBase()
                {
                    isSuccess = false,
                    message = ex.Message,
                    code = 500
                };
                return Ok(bad);
            }
        }
        [HttpPost("digital-percent")]
        public async Task<IActionResult> digitalPercent(int type, int quarter, int year)
        {
            try
            {
                if (type == 1)
                {
                    var data = await DigitalPercentQuarter(quarter, year);
                    return Ok(data);
                }
                else
                {
                    var data = await DigitalPercentYear(year);
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                var bad = new ResponseBase()
                {
                    isSuccess = false,
                    message = ex.Message,
                    code = 500
                };
                return Ok(bad);
            }
        }
        [HttpPost("country-percent")]
        public async Task<IActionResult> countryPercent(int type, int quarter, int year)
        {
            try
            {
                if (type == 1)
                {
                    var data = await CountryPercentQuarter(quarter, year);
                    return Ok(data);
                }
                else
                {
                    var data = await CountryPercentYear(year);
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                var bad = new ResponseBase()
                {
                    isSuccess = false,
                    message = ex.Message,
                    code = 500
                };
                return Ok(bad);
            }
        }
        [HttpPost("price-name-percent")]
        public async Task<IActionResult> priceNamePercent(int type, int quarter, int year)
        {
            try
            {
                if (type == 1)
                {
                    var data = await PriceQuarter(year, quarter);
                    return Ok(data);
                }
                else
                {
                    var data = await PriceYear(year);
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                var bad = new ResponseBase()
                {
                    isSuccess = false,
                    message = ex.Message,
                    code = 500
                };
                return Ok(bad);
            }
        }
        [HttpPost("youtube-percent")]
        public async Task<IActionResult> youtubePercent(int type, int quarter, int year)
        {
            try
            {
                if (type == 1)
                {
                    var data = await YoutubeQuarter(year, quarter);
                    return Ok(data);
                }
                else
                {
                    var data = await YoutubeYear(year);
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                var bad = new ResponseBase()
                {
                    isSuccess = false,
                    message = ex.Message,
                    code = 500
                };
                return Ok(bad);
            }
        }
        protected async Task<ResponseData<StatisticSumModel>> DigitalSumMonth(int year, string? language)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new List<StatisticDataSumModel>();
            var data = new List<DigitalMonthSumModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.Digital_Month_Sum(year);
            }
            else
            {
                data = await _statisticService.Digital_Month_Sum_Singer(year, artistName);
            }
            var providers = data.Select(x => x.digitalServiceProvider).Distinct().ToList();
            var months = Enumerable.Range(1, 12).ToList();
            foreach (var provider in providers)
            {
                var seriesData = new StatisticDataSumModel
                {
                    Name = provider,
                    Data = new List<long>()
                };
                foreach (var month in months)
                {
                    // Lấy dữ liệu cho từng tháng và provider
                    var sum = data
                        .Where(x => x.digitalServiceProvider == provider && x.month == month)
                        .Select(x => x.sum)
                        .FirstOrDefault();
                    seriesData.Data.Add(sum == 0 ? 0 : (isAdminClaim == "True" ? sum : _commonService.GetNetSinger(revenuePercentage, sum)));
                }

                dataChart.Add(seriesData);
            }

            var res = new ResponseData<StatisticSumModel>()
            {
                data = new StatisticSumModel()
                {
                    Data = dataChart,
                    Categories = language == null ? MonthNames.GetMonths("vi") : MonthNames.GetMonths(language)
                }
            };
            return res;
        }
        protected async Task<ResponseData<StatisticSumModel>> DigitalSumQuarter(int year, string? language)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new List<StatisticDataSumModel>();
            var data = new List<DigitalQuarterSumModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.Digital_Quarter_Sum(year);
            }
            else
            {
                data = await _statisticService.Digital_Quarter_Sum_Singer(year, artistName);
            }
            var providers = data.Select(x => x.digitalServiceProvider).Distinct().ToList();
            var quarters = Enumerable.Range(1, 4).ToList();
            foreach (var provider in providers)
            {
                var seriesData = new StatisticDataSumModel
                {
                    Name = provider,
                    Data = new List<long>()
                };
                foreach (var quarter in quarters)
                {
                    var sum = data
                        .Where(x => x.digitalServiceProvider == provider && x.quarter == quarter)
                        .Select(x => x.sum)
                        .FirstOrDefault();
                    seriesData.Data.Add(sum == 0 ? 0 : (isAdminClaim == "True" ? sum : _commonService.GetNetSinger(revenuePercentage, sum)));
                }

                dataChart.Add(seriesData);
            }

            var res = new ResponseData<StatisticSumModel>()
            {
                data = new StatisticSumModel()
                {
                    Data = dataChart,
                    Categories = language == null ? MonthNames.GetQuarter("vi") : MonthNames.GetQuarter(language)
                }
            };
            return res;
        }
        protected async Task<ResponseData<StatisticSumModel>> DigitalSumYear()
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new List<StatisticDataSumModel>();
            var data = new List<DigitalYearSumModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.Digital_Year_Sum();
            }
            else
            {
                data = await _statisticService.Digital_Year_Sum_Singer(artistName);
            }
            var providers = data.Select(x => x.digitalServiceProvider).Distinct().ToList();
            var years = Enumerable.Range((DateTime.Now.Year - 4), 5).ToList();
            foreach (var provider in providers)
            {
                var seriesData = new StatisticDataSumModel
                {
                    Name = provider,
                    Data = new List<long>()
                };
                foreach (var year in years)
                {
                    var sum = data
                        .Where(x => x.digitalServiceProvider == provider && x.quarterYear == year)
                        .Select(x => x.sum)
                        .FirstOrDefault();
                    seriesData.Data.Add(sum == 0 ? 0 : (isAdminClaim == "True" ? sum : _commonService.GetNetSinger(revenuePercentage, sum)));
                }

                dataChart.Add(seriesData);
            }

            var res = new ResponseData<StatisticSumModel>()
            {
                data = new StatisticSumModel()
                {
                    Data = dataChart,
                    Categories = years.ConvertAll<string>(x => x.ToString())
                }
            };
            return res;
        }
        protected async Task<ResponseData<StatisticDataPercentModel>> DigitalPercentYear(int year)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new StatisticDataPercentModel();
            var data = new List<DigitalYearPercentModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.Digital_year_Percent(year);
            }
            else
            {
                data = await _statisticService.Digital_Year_Percent_Singer(year, artistName);
            }
            if(data !=null && data.Count() > 0)
            {
                dataChart.labels = data.Select(x => x.digitalServiceProvider).ToList();
                dataChart.data = data.Select(x => (isAdminClaim == "True" ? x.sum : _commonService.GetNetSinger(revenuePercentage, x.sum))).ToList();
            }
            var res = new ResponseData<StatisticDataPercentModel>()
            {
                data = dataChart
            };
            return res;
        }
        protected async Task<ResponseData<StatisticDataPercentModel>> DigitalPercentQuarter(int quarter, int year)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new StatisticDataPercentModel();
            var data = new List<DigitalQuarterPercentModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.Digital_Quarter_Percent(quarter, year);
            }
            else
            {
                data = await _statisticService.Digital_Quarter_Percent_Singer(quarter, year, artistName);
            }
            if (data != null && data.Count() > 0)
            {
                dataChart.labels = data.Select(x => x.digitalServiceProvider).ToList();
                dataChart.data = data.Select(x => (isAdminClaim == "True" ? x.sum : _commonService.GetNetSinger(revenuePercentage, x.sum))).ToList();
            }
            var res = new ResponseData<StatisticDataPercentModel>()
            {
                data = dataChart
            };
            return res;
        }
        protected async Task<ResponseData<StatisticDataPercentModel>>CountryPercentQuarter(int quarter, int year)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new StatisticDataPercentModel();
            var data = new List<CountryPercentModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.Country_Quarter_Percent(quarter, year);
            }
            else
            {
                data = await _statisticService.Country_Quarter_Percent_Singer(quarter, year, artistName);
            }
            if (data != null && data.Count() > 0)
            {
                dataChart.labels = data.Select(x => x.countryCode).ToList();
                dataChart.data = data.Select(x => (isAdminClaim == "True" ? x.sum : _commonService.GetNetSinger(revenuePercentage, x.sum))).ToList();
            }
            var res = new ResponseData<StatisticDataPercentModel>()
            {
                data = dataChart
            };
            return res;
        }
        protected async Task<ResponseData<StatisticDataPercentModel>> CountryPercentYear(int year)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new StatisticDataPercentModel();
            var data = new List<CountryPercentModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.Country_Year_Percent(year);
            }
            else
            {
                data = await _statisticService.Country_Year_Percent_Singer(year, artistName);
            }
            if (data != null && data.Count() > 0)
            {
                dataChart.labels = data.Select(x => x.countryCode).ToList();
                dataChart.data = data.Select(x => (isAdminClaim == "True" ? x.sum : _commonService.GetNetSinger(revenuePercentage, x.sum))).ToList();
            }
            var res = new ResponseData<StatisticDataPercentModel>()
            {
                data = dataChart
            };
            return res;
        }
        protected async Task<ResponseData<StatisticDataPercentModel>> PriceQuarter(int year, int quarter)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new StatisticDataPercentModel();
            var data = new List<StatisticPriceNameModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.PriceQuarter(year, quarter);
            }
            else
            {
                data = await _statisticService.PriceQuarter_Singer(year, quarter, artistName);
            }
            if (data != null && data.Count() > 0)
            {
                dataChart.labels = data.Select(x => x.priceName).ToList();
                dataChart.data = data.Select(x => (isAdminClaim == "True" ? x.sum : _commonService.GetNetSinger(revenuePercentage, x.sum))).ToList();
            }
            var res = new ResponseData<StatisticDataPercentModel>()
            {
                data = dataChart
            };
            return res;
        }
        protected async Task<ResponseData<StatisticDataPercentModel>> PriceYear(int year)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new StatisticDataPercentModel();
            var data = new List<StatisticPriceNameModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.PriceYear(year);
            }
            else
            {
                data = await _statisticService.PriceYear_Singer(year, artistName);
            }
            if (data != null && data.Count() > 0)
            {
                dataChart.labels = data.Select(x => x.priceName).ToList();
                dataChart.data = data.Select(x => (isAdminClaim == "True" ? x.sum : _commonService.GetNetSinger(revenuePercentage, x.sum))).ToList();
            }
            var res = new ResponseData<StatisticDataPercentModel>()
            {
                data = dataChart
            };
            return res;
        }
        protected async Task<ResponseData<StatisticDataPercentModel>> YoutubeQuarter(int year, int quarter)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new StatisticDataPercentModel();
            var data = new List<StatisticYoutubeModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.YoutubeQuarter(year, quarter);
            }
            else
            {
                data = await _statisticService.YoutubeQuarter_Singer(year, quarter, artistName);
            }
            if (data != null && data.Count() > 0)
            {
                dataChart.labels = data.Select(x => x.revenue).ToList();
                dataChart.data = data.Select(x => (isAdminClaim == "True" ? x.sum : _commonService.GetNetSinger(revenuePercentage, x.sum))).ToList();
            }
            var res = new ResponseData<StatisticDataPercentModel>()
            {
                data = dataChart
            };
            return res;
        }
        protected async Task<ResponseData<StatisticDataPercentModel>> YoutubeYear(int year)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;

            var dataChart = new StatisticDataPercentModel();
            var data = new List<StatisticYoutubeModel>();
            if (isAdminClaim == "True")
            {
                data = await _statisticService.YoutubeYear(year);
            }
            else
            {
                data = await _statisticService.YoutubeYear_Singer(year, artistName);
            }
            if (data != null && data.Count() > 0)
            {
                dataChart.labels = data.Select(x => x.revenue).ToList();
                dataChart.data = data.Select(x => (isAdminClaim == "True" ? x.sum : _commonService.GetNetSinger(revenuePercentage, x.sum))).ToList();
            }
            var res = new ResponseData<StatisticDataPercentModel>()
            {
                data = dataChart
            };
            return res;
        }

    }
}