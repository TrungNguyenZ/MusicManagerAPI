using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Services;
using MusicManager.Services.TopChart;
using OfficeOpenXml;
using System.Globalization;

namespace MusicManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TopChartController : ControllerBase
    {
        private readonly ITopChartService _topChartService;
        public TopChartController(ITopChartService topChartService)
        {
            _topChartService = topChartService;
        }
        [HttpGet("Artist")]
        public async Task<IActionResult> Artist(int type, int quarter, int year, int? pageSize)
        {
            try
            {
                var res = new ResponseData<List<TopChartArtist>>();

                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                if (isAdminClaim == "False") {
                    res.message = "Bạn không có quyền sử dụng API này!";
                    res.code = 401;
                    return Ok(res);
                }
                if (type == 1)
                {
                    res.data = await _topChartService.TopChartArt_Quarter(quarter, year, pageSize ?? 5);
                }
                else
                {
                    res.data = await _topChartService.TopChartArt_Year(year, pageSize ?? 5);
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
        [HttpGet("Track")]
        public async Task<IActionResult> Track(int type, int quarter, int year, int? pageSize)
        {
            try
            {
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var res = new ResponseData<List<TopChartTrack>>();
                if (type == 1)
                {
                    if(isAdminClaim == "True")
                        res.data = await _topChartService.TopChartTrack_Quarter(quarter, year, pageSize ?? 5);
                    else
                        res.data = await _topChartService.TopChartTrack_Quarter_Singer(quarter, year, artistName, pageSize ?? 5);
                }
                else
                {
                    if (isAdminClaim == "True")
                        res.data = await _topChartService.TopChartTrack_Year(year, pageSize ?? 5);
                    else
                        res.data = await _topChartService.TopChartTrack_Year_Singer(year, artistName, pageSize ?? 5);
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

    }
}