﻿using MusicManager.Models;

namespace MusicManager.Repositories
{
    public interface ITopChartRepository
    {
        Task<List<TopChartArtist>> TopChartArt_Quarter(int quarter, int quarterYear, int pageSize = 5);
        Task<List<TopChartArtist>> TopChartArt_Year(int year, int pageSize = 5);
        Task<List<TopChartTrack>> TopChartTrack_Quarter(int quarter, int quarterYear, int pageSize = 5);
        Task<List<TopChartTrack>> TopChartTrack_Year(int year, int pageSize = 5);
        Task<List<TopChartTrack>> TopChartTrack_Quarter_Singer(int quarter, int quarterYear, string artistName, int pageSize = 5);
        Task<List<TopChartTrack>> TopChartTrack_Year_Singer(int year, string artistName, int pageSize = 5);
    }

}
