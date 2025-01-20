using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;

namespace MusicManager
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<StatisticTotalModel> StatisticTotalModels { get; set; }
        public DbSet<DigitalMonthSumModel> DigitalMonthSumModel { get; set; }
        public DbSet<DigitalQuarterSumModel> DigitalQuarterSumModel { get; set; }
        public DbSet<DigitalYearSumModel> DigitalYearSumModel { get; set; }
        public DbSet<DigitalQuarterPercentModel> DigitalQuarterPercentModel { get; set; }
        public DbSet<DigitalYearPercentModel> DigitalYearPercentModel { get; set; }
        public DbSet<CountryPercentModel> CountryPercentModel { get; set; }
        public DbSet<StatisticYoutubeModel> StatisticYoutubeModel { get; set; }
        public DbSet<StatisticPriceNameModel> StatisticPriceNameModel { get; set; }
        public DbSet<TopChartArtist> TopChartArtist { get; set; }
        public DbSet<TopChartTrack> TopChartTrack { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<DataModel> DataModels { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatisticTotalModel>().HasNoKey();
            modelBuilder.Entity<DigitalMonthSumModel>().HasNoKey();
            modelBuilder.Entity<DigitalQuarterSumModel>().HasNoKey();
            modelBuilder.Entity<DigitalYearSumModel>().HasNoKey();
            modelBuilder.Entity<DigitalQuarterPercentModel>().HasNoKey();
            modelBuilder.Entity<DigitalYearPercentModel>().HasNoKey();
            modelBuilder.Entity<CountryPercentModel>().HasNoKey();
            modelBuilder.Entity<StatisticYoutubeModel>().HasNoKey();
            modelBuilder.Entity<StatisticPriceNameModel>().HasNoKey();
            modelBuilder.Entity<TopChartArtist>().HasNoKey();
            modelBuilder.Entity<TopChartTrack>().HasNoKey();
            base.OnModelCreating(modelBuilder); 

        }
    }
}
