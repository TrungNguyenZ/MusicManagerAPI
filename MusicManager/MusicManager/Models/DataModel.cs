using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MusicManager.Models
{
    public class DataModel
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string marketingOwner { get; set; }
        public string artistName { get; set; }
        public string projectTitle { get; set; }
        public string catalogueNumber { get; set; }
        public string isrc { get; set; }
        public string catalogueTitle { get; set; }
        public string reportedMon { get; set; }
        public string digitalServiceProvider { get; set; }
        public string countryCode { get; set; }
        public string countryDescription{ get; set; }
        public string priceName{ get; set; }
        public string revenueTypeDesc{ get; set; }
        public int sale{ get; set; }
        public decimal exchRate { get; set; }
        public decimal grossIncome { get; set; }
        public decimal distributionFees { get; set; }
        public decimal netIncome { get; set; }
        public decimal netIncomeCompany { get; set; }
        public decimal netIncomeSinger { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int quarter { get; set; }
        public int quarterYear { get; set; }
    }
}
