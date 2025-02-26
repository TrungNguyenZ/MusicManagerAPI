using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicManager.Const;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Services;
using MusicManager.Services.Redis;
using OfficeOpenXml;
using System;
using System.Globalization;
using System.Text.Json;

namespace MusicManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;
        private readonly ICommonService _commonService;
        private readonly IRedisService _redisService;
        public StatisticController(
            IStatisticService statisticService,
            ICommonService commonService,
            IRedisService redisService
            )
        {
            _statisticService = statisticService;
            _commonService = commonService;
            _redisService = redisService;
        }
        [Authorize]
        [HttpGet("total")]
        public async Task<IActionResult> Total(int quarter, int quarterYear)
        {
            try
            {
                var res = new ResponseData<StatisticTotalModel>();
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var revenuePercentage = User.FindFirst("revenuePercentage").Value;
                var isEnterprise = User.FindFirst("isEnterprise").Value;

                var cacheKey = isAdminClaim == "True"
                    ? $"Total_{quarter}_{quarterYear}"
                    : $"Total_{quarter}_{quarterYear}_{artistName}";

                var dataRedis = await _redisService.GetAsync(cacheKey);
                if (dataRedis == null)
                {
                    res.data = isAdminClaim == "True"
                        ? await _statisticService.GetTotal(quarter, quarterYear)
                        : await _statisticService.GetTotalBySinger(quarter, quarterYear, artistName, Double.Parse(revenuePercentage), isEnterprise);

                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(res.data));
                }
                else
                {
                    res.data = JsonSerializer.Deserialize<StatisticTotalModel>(dataRedis);
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

        [HttpGet("top")]
        public async Task<IActionResult> StatisticTop(int type, int quarter, int year)
        {
            try
            {
                var res = new ResponseData<StatisticTop>();
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var revenuePercentage = User.FindFirst("revenuePercentage").Value;
                var isEnterprise = User.FindFirst("isEnterprise").Value;
                var cacheKey = isAdminClaim == "True"
                    ? $"Top_{type}_{quarter}_{year}"
                    : $"Top_{type}_{quarter}_{year}_{artistName}";

                var dataRedis = await _redisService.GetAsync(cacheKey);
                if (dataRedis == null)
                {
                    res.data = isAdminClaim == "True"
                        ? await _statisticService.StatisticTop(type, quarter, year)
                        : await _statisticService.StatisticTop_Singer(type, quarter, year, artistName, Double.Parse(revenuePercentage), isEnterprise);

                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(res.data));
                }
                else
                {
                    res.data = JsonSerializer.Deserialize<StatisticTop>(dataRedis);
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

        [HttpGet("digital-total")]
        public async Task<IActionResult> digitalSum(int type, int year, string? language)
        {
            try
            {
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var cacheKey = "";

                if (type == 1)
                {
                    cacheKey = $"DigitalSumMonth_{year}_{language}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticSumModel>>(dataRedis));

                    var data = await DigitalSumMonth(year, language);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
                    return Ok(data);
                }
                else if (type == 2)
                {
                    cacheKey = $"DigitalSumQuarter_{year}_{language}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticSumModel>>(dataRedis));

                    var data = await DigitalSumQuarter(year, language);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
                    return Ok(data);
                }
                else
                {
                    cacheKey = $"DigitalSumYear_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticSumModel>>(dataRedis));

                    var data = await DigitalSumYear();
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
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

        [HttpGet("digital-total-mobile")]
        public async Task<IActionResult> digitalSumMobile(int type, int year, string? language)
        {
            try
            {
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var cacheKey = "";

                if (type == 1)
                {
                    cacheKey = $"DigitalSumMonthMobile_{year}_{language}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<List<DigitalSumMobileModel>>>(dataRedis));

                    var data = await DigitalSumMonthMobile(year, language);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
                    return Ok(data);
                }
                else if (type == 2)
                {
                    cacheKey = $"DigitalSumQuarterMobile_{year}_{language}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<List<DigitalSumMobileModel>>>(dataRedis));

                    var data = await DigitalSumQuarterMobile(year, language);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
                    return Ok(data);
                }
                else
                {
                    cacheKey = $"DigitalSumYearMobile_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<List<DigitalSumMobileModel>>>(dataRedis));

                    var data = await DigitalSumYearMobile();
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
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

        [HttpGet("digital-percent")]
        public async Task<IActionResult> digitalPercent(int type, int quarter, int year)
        {
            try
            {
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var cacheKey = "";

                if (type == 1)
                {
                    cacheKey = $"DigitalPercentQuarter_{quarter}_{year}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticDataPercentModel>>(dataRedis));

                    var data = await DigitalPercentQuarter(quarter, year);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
                    return Ok(data);
                }
                else
                {
                    cacheKey = $"DigitalPercentYear_{year}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticDataPercentModel>>(dataRedis));

                    var data = await DigitalPercentYear(year);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
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

        [HttpGet("country-percent")]
        public async Task<IActionResult> countryPercent(int type, int quarter, int year)
        {
            try
            {
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var cacheKey = "";

                if (type == 1)
                {
                    cacheKey = $"CountryPercentQuarter_{quarter}_{year}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticDataPercentModel>>(dataRedis));

                    var data = await CountryPercentQuarter(quarter, year);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
                    return Ok(data);
                }
                else
                {
                    cacheKey = $"CountryPercentYear_{year}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticDataPercentModel>>(dataRedis));

                    var data = await CountryPercentYear(year);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
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

        [HttpGet("price-name-percent")]
        public async Task<IActionResult> priceNamePercent(int type, int quarter, int year)
        {
            try
            {
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var cacheKey = "";

                if (type == 1)
                {
                    cacheKey = $"PriceQuarter_{year}_{quarter}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticDataPercentModel>>(dataRedis));

                    var data = await PriceQuarter(year, quarter);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
                    return Ok(data);
                }
                else
                {
                    cacheKey = $"PriceYear_{year}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticDataPercentModel>>(dataRedis));

                    var data = await PriceYear(year);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
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

        [HttpGet("youtube-percent")]
        public async Task<IActionResult> youtubePercent(int type, int quarter, int year)
        {
            try
            {
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var cacheKey = "";

                if (type == 1)
                {
                    cacheKey = $"YoutubeQuarter_{year}_{quarter}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticDataPercentModel>>(dataRedis));

                    var data = await YoutubeQuarter(year, quarter);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
                    return Ok(data);
                }
                else
                {
                    cacheKey = $"YoutubeYear_{year}_{isAdminClaim}_{artistName}";
                    var dataRedis = await _redisService.GetAsync(cacheKey);
                    if (dataRedis != null)
                        return Ok(JsonSerializer.Deserialize<ResponseData<StatisticDataPercentModel>>(dataRedis));

                    var data = await YoutubeYear(year);
                    await _redisService.SetAsync(cacheKey, JsonSerializer.Serialize(data));
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
        protected async Task<ResponseData<List<DigitalSumMobileModel>>> DigitalSumMonthMobile(int year, string? language)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Digital_Month_Sum(year)
                : await _statisticService.Digital_Month_Sum_Singer(year, artistName);

            var providers = data.Select(x => x.digitalServiceProvider).Distinct().ToList();
            var months = Enumerable.Range(1, 12);

            var result = months.Select(month => new DigitalSumMobileModel
            {
                key = language == null
                        ? $"Tháng {month}"
                        : MonthNames.GetMonths(language)[month - 1],
                revenues = providers.Select(provider =>
                {
                    var sum = data
                        .Where(x => x.digitalServiceProvider == provider && x.month == month)
                        .Select(x => x.sum)
                        .FirstOrDefault();
                    return new RevenueModel
                    {
                        platformName = provider,
                        value = _commonService.GetNetEnterprise(sum) == 0 ? 0 : (isAdmin ? _commonService.GetNetEnterprise(sum) : _commonService.GetNetSinger(revenuePercentage, sum, isEnterprise))
                    };
                }).ToList()
            }).ToList();

            return new ResponseData<List<DigitalSumMobileModel>> { data = result };
        }

        protected async Task<ResponseData<List<DigitalSumMobileModel>>> DigitalSumQuarterMobile(int year, string? language)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Digital_Quarter_Sum(year)
                : await _statisticService.Digital_Quarter_Sum_Singer(year, artistName);

            var providers = data.Select(x => x.digitalServiceProvider).Distinct().ToList();
            var quarters = Enumerable.Range(1, 4);

            var result = quarters.Select(quarter => new DigitalSumMobileModel
            {
                key = language == null
                        ? $"Quý {quarter}"
                        : MonthNames.GetQuarter(language)[quarter - 1],
                revenues = providers.Select(provider =>
                {
                    var sum = data
                        .Where(x => x.digitalServiceProvider == provider && x.quarter == quarter)
                        .Select(x => x.sum)
                        .FirstOrDefault();
                    return new RevenueModel
                    {
                        platformName = provider,
                        value = sum == 0
                                  ? 0
                                  : (isAdmin ? _commonService.GetNetEnterprise(sum) : _commonService.GetNetSinger(revenuePercentage, sum, isEnterprise))
                    };
                }).ToList()
            }).ToList();

            return new ResponseData<List<DigitalSumMobileModel>> { data = result };
        }

        protected async Task<ResponseData<List<DigitalSumMobileModel>>> DigitalSumYearMobile()
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Digital_Year_Sum()
                : await _statisticService.Digital_Year_Sum_Singer(artistName);

            var providers = data.Select(x => x.digitalServiceProvider).Distinct().ToList();
            var years = Enumerable.Range(DateTime.Now.Year - 4, 5);

            var result = years.Select(year => new DigitalSumMobileModel
            {
                key = year.ToString(),
                revenues = providers.Select(provider =>
                {
                    var sum = data
                        .Where(x => x.digitalServiceProvider == provider && x.quarterYear == year)
                        .Select(x => x.sum)
                        .FirstOrDefault();
                    return new RevenueModel
                    {
                        platformName = provider,
                        value = sum == 0
                                  ? 0
                                  : (isAdmin ? _commonService.GetNetEnterprise(sum) : _commonService.GetNetSinger(revenuePercentage, sum, isEnterprise))
                    };
                }).ToList()
            }).ToList();

            return new ResponseData<List<DigitalSumMobileModel>> { data = result };
        }


        protected async Task<ResponseData<StatisticSumModel>> DigitalSumMonth(int year, string? language)
        {
            var userClaims = new
            {
                IsAdmin = User.FindFirst("isAdmin")?.Value == "True",
                ArtistName = User.FindFirst("artistName")?.Value,
                RevenuePercentage = User.FindFirst("revenuePercentage")?.Value,
                IsEnterprise = User.FindFirst("isEnterprise")?.Value
            };

            var data = userClaims.IsAdmin
                ? await _statisticService.Digital_Month_Sum(year)
                : await _statisticService.Digital_Month_Sum_Singer(year, userClaims.ArtistName);

            var providers = data.Select(x => x.digitalServiceProvider).Distinct();
            var months = Enumerable.Range(1, 12);

            var dataChart = providers.Select(provider => new StatisticDataSumModel
            {
                Name = provider,
                Data = months.Select(month =>
                {
                    var sum = data.FirstOrDefault(x => x.digitalServiceProvider == provider && x.month == month)?.sum ?? 0;
                    return userClaims.IsAdmin ? _commonService.GetNetEnterprise(sum) :  _commonService.GetNetSinger(userClaims.RevenuePercentage, sum, userClaims.IsEnterprise);
                }).ToList()
            }).OrderBy(x=>x.Name).ToList();

            return new ResponseData<StatisticSumModel>
            {
                data = new StatisticSumModel
                {
                    Data = dataChart,
                    Categories = MonthNames.GetMonths(language ?? "vi")
                }
            };
        }

        protected async Task<ResponseData<StatisticSumModel>> DigitalSumQuarter(int year, string? language)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Digital_Quarter_Sum(year)
                : await _statisticService.Digital_Quarter_Sum_Singer(year, artistName);

            var providers = data.Select(x => x.digitalServiceProvider).Distinct().ToList();
            var quarters = Enumerable.Range(1, 4).ToList();
            var dataChart = new List<StatisticDataSumModel>();

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

                    long value = sum == 0
                        ? 0
                        : isAdmin
                            ? _commonService.GetNetEnterprise(sum)
                            : _commonService.GetNetSinger(revenuePercentage, sum, isEnterprise);

                    seriesData.Data.Add(value);
                }

                dataChart.Add(seriesData);
            }

            return new ResponseData<StatisticSumModel>
            {
                data = new StatisticSumModel
                {
                    Data = dataChart,
                    Categories = language == null ? MonthNames.GetQuarter("vi") : MonthNames.GetQuarter(language)
                }
            };
        }

        protected async Task<ResponseData<StatisticSumModel>> DigitalSumYear()
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Digital_Year_Sum()
                : await _statisticService.Digital_Year_Sum_Singer(artistName);

            var providers = data.Select(x => x.digitalServiceProvider).Distinct().ToList();
            var years = Enumerable.Range(DateTime.Now.Year - 4, 5).ToList();
            var dataChart = new List<StatisticDataSumModel>();

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

                    long value = sum == 0
                        ? 0
                        : isAdmin
                            ? _commonService.GetNetEnterprise(sum)
                            : _commonService.GetNetSinger(revenuePercentage, sum, isEnterprise);

                    seriesData.Data.Add(value);
                }

                dataChart.Add(seriesData);
            }

            return new ResponseData<StatisticSumModel>
            {
                data = new StatisticSumModel
                {
                    Data = dataChart,
                    Categories = years.ConvertAll(x => x.ToString())
                }
            };
        }

        protected async Task<ResponseData<StatisticDataPercentModel>> DigitalPercentYear(int year)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Digital_year_Percent(year)
                : await _statisticService.Digital_Year_Percent_Singer(year, artistName);

            var dataChart = new StatisticDataPercentModel();

            if (data != null && data.Any())
            {
                dataChart.labels = data.Select(x => x.digitalServiceProvider).ToList();
                dataChart.data = data.Select(x =>
                    isAdmin
                        ? _commonService.GetNetEnterprise(x.sum)
                        : _commonService.GetNetSinger(revenuePercentage, x.sum, isEnterprise)
                ).ToList();
            }

            return new ResponseData<StatisticDataPercentModel>
            {
                data = dataChart
            };
        }

        protected async Task<ResponseData<StatisticDataPercentModel>> DigitalPercentQuarter(int quarter, int year)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Digital_Quarter_Percent(quarter, year)
                : await _statisticService.Digital_Quarter_Percent_Singer(quarter, year, artistName);

            var dataChart = new StatisticDataPercentModel();

            if (data != null && data.Any())
            {
                dataChart.labels = data.Select(x => x.digitalServiceProvider).ToList();
                dataChart.data = data.Select(x => isAdmin
                    ? _commonService.GetNetEnterprise(x.sum)
                    : _commonService.GetNetSinger(revenuePercentage, x.sum, isEnterprise)).ToList();
            }

            return new ResponseData<StatisticDataPercentModel> { data = dataChart };
        }
        protected async Task<ResponseData<StatisticDataPercentModel>> CountryPercentQuarter(int quarter, int year)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Country_Quarter_Percent(quarter, year)
                : await _statisticService.Country_Quarter_Percent_Singer(quarter, year, artistName);

            var dataChart = new StatisticDataPercentModel();

            if (data != null && data.Any())
            {
                dataChart.labels = data.Select(x => x.countryCode).ToList();
                dataChart.data = data.Select(x => isAdmin
                    ? _commonService.GetNetEnterprise(x.sum)
                    : _commonService.GetNetSinger(revenuePercentage, x.sum, isEnterprise)).ToList();
            }

            return new ResponseData<StatisticDataPercentModel> { data = dataChart };
        }

        protected async Task<ResponseData<StatisticDataPercentModel>> CountryPercentYear(int year)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.Country_Year_Percent(year)
                : await _statisticService.Country_Year_Percent_Singer(year, artistName);

            var dataChart = new StatisticDataPercentModel();

            if (data != null && data.Any())
            {
                dataChart.labels = data.Select(x => x.countryCode).ToList();
                dataChart.data = data.Select(x => isAdmin
                    ? _commonService.GetNetEnterprise(x.sum)
                    : _commonService.GetNetSinger(revenuePercentage, x.sum, isEnterprise)).ToList();
            }

            return new ResponseData<StatisticDataPercentModel> { data = dataChart };
        }

        protected async Task<ResponseData<StatisticDataPercentModel>> PriceQuarter(int year, int quarter)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.PriceQuarter(year, quarter)
                : await _statisticService.PriceQuarter_Singer(year, quarter, artistName);

            var dataChart = new StatisticDataPercentModel();
            if (data != null && data.Any())
            {
                dataChart.labels = data.Select(x => x.priceName).ToList();
                dataChart.data = data.Select(x => isAdmin
                    ? _commonService.GetNetEnterprise(x.sum)
                    : _commonService.GetNetSinger(revenuePercentage, x.sum, isEnterprise)
                ).ToList();
            }

            return new ResponseData<StatisticDataPercentModel> { data = dataChart };
        }

        protected async Task<ResponseData<StatisticDataPercentModel>> PriceYear(int year)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.PriceYear(year)
                : await _statisticService.PriceYear_Singer(year, artistName);

            var dataChart = new StatisticDataPercentModel();
            if (data != null && data.Any())
            {
                dataChart.labels = data.Select(x => x.priceName).ToList();
                dataChart.data = data.Select(x => isAdmin
                    ? _commonService.GetNetEnterprise(x.sum)
                    : _commonService.GetNetSinger(revenuePercentage, x.sum, isEnterprise)
                ).ToList();
            }

            return new ResponseData<StatisticDataPercentModel> { data = dataChart };
        }

        protected async Task<ResponseData<StatisticDataPercentModel>> YoutubeQuarter(int year, int quarter)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.YoutubeQuarter(year, quarter)
                : await _statisticService.YoutubeQuarter_Singer(year, quarter, artistName);

            var dataChart = new StatisticDataPercentModel();
            if (data != null && data.Any())
            {
                dataChart.labels = data.Select(x => x.revenue).ToList();
                dataChart.data = data.Select(x => isAdmin
                    ? _commonService.GetNetEnterprise(x.sum)
                    : _commonService.GetNetSinger(revenuePercentage, x.sum, isEnterprise)
                ).ToList();
            }

            return new ResponseData<StatisticDataPercentModel> { data = dataChart };
        }

        protected async Task<ResponseData<StatisticDataPercentModel>> YoutubeYear(int year)
        {
            bool isAdmin = User.FindFirst("isAdmin")?.Value == "True";
            string isEnterprise = User.FindFirst("isEnterprise")?.Value;
            string artistName = User.FindFirst("artistName")?.Value;
            string revenuePercentage = User.FindFirst("revenuePercentage")?.Value;

            var data = isAdmin
                ? await _statisticService.YoutubeYear(year)
                : await _statisticService.YoutubeYear_Singer(year, artistName);

            var dataChart = new StatisticDataPercentModel();
            if (data != null && data.Any())
            {
                dataChart.labels = data.Select(x => x.revenue).ToList();
                dataChart.data = data.Select(x => isAdmin
                    ? _commonService.GetNetEnterprise(x.sum)
                    :  _commonService.GetNetSinger(revenuePercentage, x.sum, isEnterprise)
                ).ToList();
            }

            return new ResponseData<StatisticDataPercentModel> { data = dataChart };
        }

    }
}