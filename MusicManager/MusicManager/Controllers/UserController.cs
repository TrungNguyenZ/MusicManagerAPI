using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Repositories;
using MusicManager.Services;
using MusicManager.Services.Redis;
using Org.BouncyCastle.Asn1.Ocsp;
using StackExchange.Redis;

namespace MusicManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ICommonService _commonService;
        private readonly IRedisService _redisService;

        // Bộ nhớ lưu refresh token
        private static readonly Dictionary<string, List<string>> RefreshTokens = new Dictionary<string, List<string>>();

        public UserController(
            UserManager<ApplicationUser> userManager, 
            ITokenService tokenService, 
            IRefreshTokenService refreshTokenService,
            ICommonService commonService,
            IRedisService redisService
            )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _commonService = commonService;
            _redisService = redisService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var res = new ResponseData<ResponseToken>();
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                res.code = 401;
                res.message = "Tài khoản đăng nhập không đúng!";
                return Ok(res);
            }
            //if (!user.Active)
            //{
            //    res.code = 402;
            //    res.message = "Tài khoản của bạn chưa được kích hoạt!";
            //    return Ok(res);
            //}
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Lưu refresh token vào danh sách
                //if (!RefreshTokens.ContainsKey(user.UserName))
                //{
                //    RefreshTokens[user.UserName] = new List<string>();
                //}
                //RefreshTokens[user.UserName].Add(refreshToken); 
                var refreshTokenEntity = new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(30) // Thời gian hết hạn
                };

                _refreshTokenService.Create(refreshTokenEntity);

                res.data = new ResponseToken
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    FullName = user.Name,
                    IsAdmin = user.IsAdmin,
                    ImageUrl = user.ImageUrl
                };
            }
            else
            {
                res.code = 401;
                res.message = "Mật khẩu đăng nhập không đúng!";
                return Ok(res);
            }
            return Ok(res);

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
                var username = principal.Identity.Name;
                var user = await _userManager.FindByNameAsync(username);

                var storedToken = _refreshTokenService.GetAll().FirstOrDefault(t =>
               t.UserId == user.Id &&
               t.Token == request.RefreshToken &&
               t.ExpiresAt > DateTime.UtcNow);
                // Kiểm tra xem user có refresh token này không
                if (storedToken == null)
                {
                    var rs = new ResponseBase()
                    {
                        code = 401,
                        message = "Phiên đăng nhập hết hạn"
                    };
                    return Unauthorized(rs);
                }

                var userData = await _userManager.FindByNameAsync(username);
                var newAccessToken = await _tokenService.GenerateAccessToken(userData);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                storedToken.Token = newRefreshToken;
                storedToken.ExpiresAt = DateTime.UtcNow.AddDays(30);
                _refreshTokenService.Update(storedToken);
                // Cập nhật danh sách refresh token
                //savedRefreshTokens.Remove(request.RefreshToken); // Xóa token cũ
                //savedRefreshTokens.Add(newRefreshToken); // Thêm token mới

                var res = new ResponseData<ResponseToken>()
                {
                    data = new ResponseToken
                    {
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken,
                        FullName = userData.Name,
                        IsAdmin = userData.IsAdmin
                    }
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        // API thêm tài khoản
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserRequest model)
        {
            var res = new ResponseBase();

            try
            {
                if (await _userManager.FindByNameAsync(model.Username) != null)
                {
                    res.code = 409;
                    res.message = "Tài khoản đã tồn tại!";
                    return Ok(res);
                }

                string imageUrl = null;
                if (model.Image != null)
                {
                    imageUrl = await _commonService.SaveImageAsync(model.Image);
                }

                var input = new ApplicationUser
                {
                    UserName = model.Username,
                    PhoneNumber = model.Phone,
                    Email = model.Email,
                    ArtistName = model.ArtistName,
                    Name = model.Name,
                    RevenuePercentage = model.RevenuePercentage,
                    IsAdmin = model.IsAdmin,
                    IsEnterprise = model.IsEnterprise,
                    ImageUrl = imageUrl,
                    Active = false
                };
                var result = await _userManager.CreateAsync(input, model.Password);
                if (result.Succeeded)
                {
                    res.message = "Tạo tài khoản thành công!";
                    _redisService.ClearCacheContaining("TableRevenue");
                    return Ok(res);
                }
                else
                {
                    res.message = "Tạo tài khoản thất bại!";
                    res.code = 400;
                    res.isSuccess = false;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.code = 500;
                res.message = $"Lỗi: {ex.Message}";
                res.isSuccess = false;
                return Ok(res);
            }
        }

        // API sửa tài khoản
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserRequest model)
        {
            var res = new ResponseBase();
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    res.code = 410;
                    res.message = "Tài khoản không tồn tại!";
                    return Ok(res);
                }

                user.Email = model.Email;
                user.IsAdmin = model.IsAdmin;
                user.IsEnterprise = model.IsEnterprise;
                user.ArtistName = model.ArtistName;
                user.PhoneNumber = model.Phone;
                user.RevenuePercentage = model.RevenuePercentage;
                user.UserName = model.Username;
                user.Name = model.Name;

                // Cập nhật ảnh nếu có
                if (model.Image != null)
                {
                    string imageUrl = await _commonService.SaveImageAsync(model.Image);
                    user.ImageUrl = imageUrl;
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    res.message = "Cập nhật tài khoản thành công.";
                    _redisService.ClearCacheContaining("TableRevenue");
                    return Ok(res);
                }
                else
                {
                    res.message = "Cập nhật tài khoản thất bại!";
                    res.code = 400;
                    res.isSuccess = false;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.code = 500;
                res.message = $"Lỗi: {ex.Message}";
                res.isSuccess = false;
                return Ok(res);
            }
        }
        [HttpPut("approve")]
        public async Task<IActionResult> approve(string id, bool active)
        {
            var res = new ResponseBase();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                res.code = 410;
                res.message = "Tài khoản không tồn tại!";
                return Ok(res);
            }
            user.Active = active;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                res.message = "Phê duyệt thành công!";
                return Ok(res);
            }
            else
            {
                res.message = "Phê duyệt tài khoản thất bại!";
                res.code = 400;
                res.isSuccess = false;
                return Ok(res);
            }
        }

        // API xóa tài khoản
        [HttpGet("delete")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var res = new ResponseBase();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(id);
                if (user == null)
                {
                    res.code = 410;
                    res.message = "Tài khoản không tồn tại!";
                    return Ok(res);
                }
            }
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                res.message = "Xoá tài khoản thành công!";
                _redisService.ClearCacheContaining("TableRevenue");
                return Ok(res);
            }
            else
            {
                res.message = "Cập nhật khoản thất bại!";
                res.code = 400;
                res.isSuccess = false;
                return Ok(res);
            }
        }
        [HttpGet("getList")]
        public async Task<IActionResult> GetList()
        {
            try
            {
                var users = _userManager.Users.OrderByDescending(x => x.Id).ToList();
                
                // Không cần xử lý ImageUrl null vì đã được khai báo là nullable trong model
                
                var res = new ResponseData<List<ApplicationUser>>()
                {
                    data = users
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                var errorRes = new ResponseBase()
                {
                    isSuccess = false,
                    code = 500,
                    message = $"Lỗi khi lấy danh sách người dùng: {ex.Message}"
                };
                return Ok(errorRes);
            }
        }
        [Authorize]
        [HttpGet("Info")]
        public async Task<IActionResult> Info()
        {
            try
            {
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                var user = await _userManager.FindByNameAsync(name);
                
                if (user == null)
                {
                    return Ok(new ResponseBase
                    {
                        isSuccess = false,
                        code = 404,
                        message = "Không tìm thấy thông tin người dùng"
                    });
                }
                
                var res = new ResponseData<object>()
                {
                    data = new
                    {
                        id = user.Id,
                        username = user.UserName,
                        name = user.Name,
                        email = user.Email,
                        phone = user.PhoneNumber,
                        artistName = user.ArtistName,
                        revenuePercentage = user.RevenuePercentage,
                        isAdmin = user.IsAdmin,
                        isEnterprise = user.IsEnterprise,
                        imageUrl = user.ImageUrl ?? "" // Trả về chuỗi rỗng nếu ImageUrl là null
                    }
                };
                
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(new ResponseBase
                {
                    isSuccess = false,
                    code = 500,
                    message = $"Lỗi: {ex.Message}"
                });
            }
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasword(ChangePassword input)
        {
            var res = new ResponseBase();
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
            {
                res.code = 410;
                res.message = "Tài khoản không tồn tại!";
                return Ok(res);
            }
            var result = await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);
            if (!result.Succeeded)
            {
                res.code = 411;
                res.message = "Mật khẩu không chính xác!";
                return Ok(res);
            }
            else
            {
                res.message = "Đổi mật khẩu thành công!";
            }
            return Ok(res);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassword input)
        {
            var res = new ResponseBase();
            var user = await _userManager.FindByIdAsync(input.Id);
            if (user == null)
            {
                res.code = 410;
                res.message = "Tài khoản không tồn tại!";
                return Ok(res);
            }

            // Tạo token đặt lại mật khẩu
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Đặt lại mật khẩu
            var result = await _userManager.ResetPasswordAsync(user, resetToken, input.NewPassword);
            if (!result.Succeeded)
            {
                res.code = 411;
                res.message = "Đặt lại mật khẩu thất bại!";
                return Ok(res);
            }

            res.code = 200;
            res.message = "Đặt lại mật khẩu thành công!";
            return Ok(res);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(LoginRequest input)
        {
            var res = new ResponseBase();
            res.code = 200;
            res.message = "Đăng ký thành công!";
            return Ok(res);
        }
        [HttpGet("reset-password-user")]
        public async Task<IActionResult> ResetPasswordUser(string email)
        {
            var res = new ResponseBase();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                res.code = 410;
                res.message = "Tài khoản không tồn tại!";
                return Ok(res);
            }

            // Tạo token đặt lại mật khẩu
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Đặt lại mật khẩu
            var newPassword = GenerateRandomPassword();
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
            {
                res.code = 411;
                res.message = "Đặt lại mật khẩu thất bại!";
                return Ok(res);
            }

            res.code = 200;
            res.message = "Mật khẩu mới đã được gửi vào Email của bạn.";
            _commonService.SendEmailAsync(new List<string> { email }, "Đặt lại mật khẩu", $"Mật khẩu mới của bạn là: {newPassword}");
            return Ok(res);
        }
        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            using (var rng = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[length];
                rng.GetBytes(buffer);
                var password = new char[length];
                for (int i = 0; i < length; i++)
                {
                    password[i] = validChars[buffer[i] % validChars.Length];
                }
                return new string(password);
            }
        }
        [Authorize]
        [HttpPost("update-personal-info")]
        public async Task<IActionResult> UpdatePersonalInfo([FromForm] UpdatePersonalInfoRequest model)
        {
            var res = new ResponseData<string>();
            try
            {
                // Lấy thông tin người dùng hiện tại
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                var user = await _userManager.FindByNameAsync(username);
                
                if (user == null)
                {
                    res.code = 404;
                    res.message = "Không tìm thấy thông tin người dùng";
                    res.isSuccess = false;
                    return Ok(res);
                }

                // Cập nhật thông tin cá nhân
                user.Name = model.Name;
                user.Email = model.Email;
                user.PhoneNumber = model.Phone;
                
                // Cập nhật ảnh đại diện nếu có
                if (model.Image != null)
                {
                    string imageUrl = await _commonService.SaveImageAsync(model.Image);
                    user.ImageUrl = imageUrl;
                }

                // Lưu thay đổi
                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    res.data = user.ImageUrl;
                    res.message = "Cập nhật thông tin cá nhân thành công";
                    return Ok(res);
                }
                else
                {
                    res.code = 400;
                    res.message = "Cập nhật thông tin cá nhân thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description));
                    res.isSuccess = false;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                res.code = 500;
                res.message = $"Lỗi: {ex.Message}";
                res.isSuccess = false;
                return Ok(res);
            }
        }
    }

}