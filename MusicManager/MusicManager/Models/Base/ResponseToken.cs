namespace MusicManager.Models.Base
{
    public class ResponseToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string FullName { get; set; }
        public bool IsAdmin { get; set; }
        public string? ImageUrl { get; set; }
    }
}
