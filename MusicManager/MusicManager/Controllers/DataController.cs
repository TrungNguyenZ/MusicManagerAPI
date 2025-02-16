using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Services;
using MusicManager.Services.Redis;
using OfficeOpenXml;
using StackExchange.Redis;
using System.Globalization;

namespace MusicManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ICommonService _commonService;
        private readonly IRedisService _redisService;
        public DataController(IDataService dataService, ICommonService commonService, IRedisService redisService)
        {
            _dataService = dataService;
            _commonService = commonService;
            _redisService = redisService;
        }
        [HttpGet("PushFCM")]
        public async Task<IActionResult> Test()
        {
            var token = await _commonService.GetAccessTokenAsync();
            await _commonService.SendNotificationToTopicAsync(token, "Đã có đối soát của Quý I năm 2025", "Vui lòng vào app kiểm tra", "all");
            var rs = new ResponseBase();
            return Ok(rs);
        }
        [HttpGet("SendEmail")]
        public async Task<IActionResult> SendEmail()
        {
            await _commonService.SendEmaiNoticationlAsync("test", "OK em ơi!");
            var rs = new ResponseBase();
            return Ok(rs);
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate()
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "SampleTemplate.xlsx");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File mẫu không tồn tại.");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "SampleTemplate.xlsx";

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("upload-excel")]
        public IActionResult UploadExcel(IFormFile file, [FromForm] int quarter, [FromForm] int year)
        {
            try
            {
                var rs = new ResponseBase()
                {
                    message = "Tệp Excel đang được xử lý, vui lòng kiểm tra sau!"
                };
                if (file == null || file.Length == 0)
                {
                    rs.code = 400;
                    rs.message = "File không hợp lệ.";
                    return Ok(rs);
                }
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    rs.code = 400;
                    rs.message = "Không phải file excel";
                    return Ok(rs);
                }

                // Lưu file vào bộ nhớ tạm
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    var fileBytes = stream.ToArray();

                    // Đẩy công việc vào hàng đợi Hangfire để xử lý nền
                    BackgroundJob.Enqueue(() =>  ProcessExcel(fileBytes, quarter, year));
                }

                return Ok(rs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [NonAction]
        public  async Task ProcessExcel(byte[] fileBytes, int quarter, int year)
        {
            try
            {
                var data = new List<Dictionary<string, string>>();

                using (var stream = new MemoryStream(fileBytes))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;
                        var colCount = worksheet.Dimension.Columns;
                        var headers = new List<string>();

                        for (int col = 1; col <= colCount; col++)
                        {
                            headers.Add(worksheet.Cells[7, col].Text);
                        }

                        for (int row = 8; row <= rowCount; row++)
                        {
                            var rowData = new Dictionary<string, string>();
                            for (int col = 1; col <= colCount; col++)
                            {
                                rowData[headers[col - 1]] = worksheet.Cells[row, col].Text;
                            }
                            data.Add(rowData);
                        }
                    }
                }

                var list = new List<DataModel>();
                foreach (var row in data)
                {
                    var dataRow = row.Where(x => !string.IsNullOrEmpty(x.Key)).ToList();
                    var netIncome = decimal.Parse(dataRow[26].Value);
                    DateTime date = DateTime.ParseExact(dataRow[8].Value, "yyyyMM", System.Globalization.CultureInfo.InvariantCulture);

                    var model = new DataModel()
                    {
                        marketingOwner = dataRow[0].Value,
                        artistName = dataRow[1].Value,
                        projectTitle = dataRow[2].Value,
                        catalogueNumber = dataRow[3].Value,
                        isrc = dataRow[6].Value,
                        catalogueTitle = dataRow[7].Value,
                        reportedMon = dataRow[8].Value,
                        digitalServiceProvider = dataRow[9].Value,
                        countryCode = dataRow[10].Value,
                        countryDescription = dataRow[11].Value,
                        priceName = dataRow[12].Value,
                        revenueTypeDesc = dataRow[13].Value,
                        sale = int.Parse(dataRow[14].Value),
                        exchRate = _commonService.ConvertDecimal(dataRow[18].Value),
                        grossIncome = _commonService.ConvertDecimal(dataRow[19].Value),
                        distributionFees = _commonService.ConvertDecimal(dataRow[21].Value),
                        netIncome = _commonService.ConvertDecimal(dataRow[26].Value),
                        netIncomeCompany = 0,
                        netIncomeSinger = 0,
                        month = date.Month,
                        year = date.Year,
                        quarter = quarter,
                        quarterYear = year
                    };

                    list.Add(model);
                }

                _dataService.AddRange(list);
                _redisService.ClearCacheContaining("_" + quarter + "_" + year);
                 var token = await _commonService.GetAccessTokenAsync();
                 await _commonService.SendNotificationToTopicAsync(token, $"Đã có đối soát của Quý {quarter} năm {year}", "Vui lòng vào app kiểm tra", "all");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xử lý file Excel: {ex.Message}");
            }
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel(int quarter, int year)
        {
            var isAdminClaim = User.FindFirst("isAdmin")?.Value;
            var artistName = User.FindFirst("artistName")?.Value;
            var revenuePercentage = User.FindFirst("revenuePercentage").Value;
            var data = new List<DataExportExcelModel>();
            if (isAdminClaim == "True")
            {
                data = await _dataService.GetDataExcel(quarter, year);
            }
            else
            {
                data = await _dataService.GetDataExcel(artistName, quarter, year);
            }
            if (data == null || !data.Any())
            {
                var res = new ResponseBase()
                {
                    code = 204,
                    message = "Không có dữ liệu!"
                };
                return Ok(res);
            }
            // Dữ liệu mẫu để xuất ra Excel

            // Tạo file Excel
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            var startRow = 2;
            worksheet.Cells[startRow, 1].Value = "Marketing owner";
            worksheet.Cells[startRow, 2].Value = "Artist name";
            worksheet.Cells[startRow, 3].Value = "Project title";
            worksheet.Cells[startRow, 4].Value = "Catalogue number";
            worksheet.Cells[startRow, 5].Value = "Isrc";
            worksheet.Cells[startRow, 6].Value = "Catalogue title";
            worksheet.Cells[startRow, 7].Value = "Report mon";
            worksheet.Cells[startRow, 8].Value = "Digital Service Provider";
            worksheet.Cells[startRow, 9].Value = "Country code";
            worksheet.Cells[startRow, 10].Value = "Country description";
            worksheet.Cells[startRow, 11].Value = "Price name";
            worksheet.Cells[startRow, 12].Value = "Revenue type Desc";
            worksheet.Cells[startRow, 13].Value = "sale";
            worksheet.Cells[startRow, 14].Value = "Net income";

            // Thêm dữ liệu vào Excel
            for (int i = 0; i < data.Count(); i++)
            {
                worksheet.Cells[i + startRow + 1, 1].Value = data[i].marketingOwner;
                worksheet.Cells[i + startRow + 1, 2].Value = data[i].artistName;
                worksheet.Cells[i + startRow + 1, 3].Value = data[i].projectTitle;
                worksheet.Cells[i + startRow + 1, 4].Value = data[i].catalogueNumber;
                worksheet.Cells[i + startRow + 1, 5].Value = data[i].isrc;
                worksheet.Cells[i + startRow + 1, 6].Value = data[i].catalogueTitle;
                worksheet.Cells[i + startRow + 1, 7].Value = data[i].reportedMon;
                worksheet.Cells[i + startRow + 1, 8].Value = data[i].digitalServiceProvider;
                worksheet.Cells[i + startRow + 1, 9].Value = data[i].countryCode;
                worksheet.Cells[i + startRow + 1, 10].Value = data[i].countryDescription;
                worksheet.Cells[i + startRow + 1, 11].Value = data[i].priceName;
                worksheet.Cells[i + startRow + 1, 12].Value = data[i].revenueTypeDesc;
                worksheet.Cells[i + startRow + 1, 13].Value = data[i].sale;
                if (isAdminClaim == "True")
                {
                    worksheet.Cells[i + startRow + 1, 14].Value = (long)data[i].netIncome;
                }
                else
                {
                    worksheet.Cells[i + startRow + 1, 14].Value = (long)_commonService.GetNetSinger(revenuePercentage, (long)data[i].netIncome);
                }
            }

            // Định dạng file Excel (tùy chọn)
            worksheet.Cells[2, 1, 2, 14].Style.Font.Bold = true; // In đậm tiêu đề cột
            worksheet.Cells.AutoFitColumns(); // Tự động căn chỉnh kích thước cột

            // Trả về file Excel
            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0;

            var fileName = "ExportedData.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return File(stream, contentType, fileName);
        }

    }
}