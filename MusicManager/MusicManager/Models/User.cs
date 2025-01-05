using Microsoft.AspNetCore.Identity;

namespace MusicManager.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class ApplicationUser : IdentityUser {
        public string Name { get; set; }
        public string ArtistName { get; set; }
        public double RevenuePercentage { get; set; }
        public bool IsAdmin { get; set; }
        public bool Active { get; set; } = false;

    }
    public class CreateUserRequest
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ArtistName { get; set; }
        public double RevenuePercentage { get; set; }
        public bool IsAdmin { get; set; } 
    }
    public class UpdateUserRequest
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; } 
        public string ArtistName { get; set; }
        public double RevenuePercentage { get; set; }
    }

}
