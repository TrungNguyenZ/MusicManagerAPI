using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Services;
using OfficeOpenXml;
using System.Globalization;

namespace MusicManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ICommonService _commonService;
        public DataController(IDataService dataService, ICommonService commonService)
        {
            _dataService = dataService;
            _commonService = commonService;
        }
        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            try
            {
                var res = new ResponseBase();
                if (file == null || file.Length == 0)
                {
                    return BadRequest("File không hợp lệ.");
                }

                var data = new List<Dictionary<string, string>>(); // Lưu dữ liệu từ Excel

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
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
                foreach (var row in data.Where((item,index)=>index > 121624).SkipLast(3))
                {
                    var dataRow = row.Where(x => x.Key != null && x.Key != "").ToList();
                    var netIncome = decimal.Parse(dataRow[26].Value);
                    string dateStr = dataRow[8].Value;
                    DateTime date = DateTime.ParseExact(dateStr, "yyyyMM", System.Globalization.CultureInfo.InvariantCulture);
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
                        sale = Int32.Parse(dataRow[14].Value),
                        exchRate = _commonService.ConvertDecimal(dataRow[18].Value),
                        grossIncome = _commonService.ConvertDecimal(dataRow[19].Value),
                        distributionFees = _commonService.ConvertDecimal(dataRow[21].Value),
                        netIncome = _commonService.ConvertDecimal(dataRow[26].Value),
                        netIncomeCompany = 0,
                        netIncomeSinger = 0,
                        month = date.Month,
                        year = date.Year,
                    };
                    list.Add(model);
                   
                }
                _dataService.AddRange(list);
                return Ok(res);
            }
            catch (Exception ex)
            {

                var res = new ResponseBase()
                {
                    isSuccess = false,
                    message = ex.Message,
                    code = 500
                };
                return Ok("OK");
            }
         
        }

    }
}