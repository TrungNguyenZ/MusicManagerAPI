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
        public async Task<IActionResult> Artist(int type, int quarter, int year)
        {
            try
            {
                var res = new ResponseData<List<TopChartArtist>>();
                if (type == 1)
                {
                    res.data = await _topChartService.TopChartArt_Quarter(quarter, year);
                }
                else
                {
                    res.data = await _topChartService.TopChartArt_Year(year);
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
        public async Task<IActionResult> Track(int type, int quarter, int year)
        {
            try
            {
                var isAdminClaim = User.FindFirst("isAdmin")?.Value;
                var artistName = User.FindFirst("artistName")?.Value;
                var res = new ResponseData<List<TopChartTrack>>();
                if (type == 1)
                {
                    if(isAdminClaim == "True")
                        res.data = await _topChartService.TopChartTrack_Quarter(quarter, year);
                    else
                        res.data = await _topChartService.TopChartTrack_Quarter_Singer(quarter, year, artistName);
                }
                else
                {
                    if (isAdminClaim == "True")
                        res.data = await _topChartService.TopChartTrack_Year(year);
                    else
                        res.data = await _topChartService.TopChartTrack_Year_Singer(year, artistName);
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