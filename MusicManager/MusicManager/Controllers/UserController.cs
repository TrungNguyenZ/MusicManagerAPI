using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Services;

namespace MusicManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        // Bộ nhớ lưu refresh token
        private static readonly Dictionary<string, string> RefreshTokens = new Dictionary<string, string>();

        public UserController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
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

                // Lưu refresh token trong bộ nhớ
                RefreshTokens[user.UserName] = refreshToken;
                res.data = new ResponseToken { AccessToken = accessToken, RefreshToken = refreshToken, FullName = user.Name, IsAdmin = user.IsAdmin };
                return Ok(res);
            }
            res.code = 401;
            res.message = "Tài khoản đăng nhập không đúng!";
            return Ok(res);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var username = principal.Identity.Name;

            if (!RefreshTokens.TryGetValue(username, out var savedRefreshToken) || savedRefreshToken != request.RefreshToken)
            {
                return Unauthorized();
            }
            var userData = await _userManager.FindByNameAsync(username);
            var newAccessToken = _tokenService.GenerateAccessToken(userData).Result;
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Cập nhật refresh token
            RefreshTokens[username] = newRefreshToken;
            var res = new ResponseData<ResponseToken>()
            {
                data = new ResponseToken { AccessToken = newAccessToken, RefreshToken = newRefreshToken, FullName = userData.Name, IsAdmin = userData.IsAdmin }
            };
            return Ok(res);
        }
        // API thêm tài khoản
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest model)
        {
            var res = new ResponseBase();

            if (await _userManager.FindByNameAsync(model.Username) != null)
            {
                res.code = 409;
                res.message = "Tài khoản đã tồn tại!";
                return Ok(res);
            }
            var input = new ApplicationUser {
                UserName = model.Username,
                PhoneNumber = model.Phone,
                Email = model.Email,
                ArtistName = model.ArtistName,
                Name = model.Name,
                RevenuePercentage = model.RevenuePercentage,
                IsAdmin = model.IsAdmin,
            };
            var result = await _userManager.CreateAsync(input, model.Password);
            if (result.Succeeded)
            {
                res.message = "Tạo tài khoản thành công!";
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

        // API sửa tài khoản
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest model)
        {
            var res = new ResponseBase();
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                res.code = 410;
                res.message = "Tài khoản không tồn tại!";
                return Ok(res);
            }

            user.Email = model.Email;
            user.IsAdmin = model.IsAdmin;
            user.ArtistName = model.ArtistName;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                res.message = "Cập nhật tài khoản thành công.";
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
                res.code = 410;
                res.message = "Tài khoản không tồn tại!";
                return Ok(res);
            }
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                res.message = "Xoá tài khoản thành công!";
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
            var res = new ResponseData<List<ApplicationUser>>()
            {
                data = _userManager.Users.OrderByDescending(x=>x.Id).ToList()
            };
            return Ok(res);
        }
        [HttpPost("ChangePasword")]
        public async Task<IActionResult> ChangePasword(ChangePassword input)
        {
            var res = new ResponseBase();
            var user = await _userManager.FindByIdAsync(input.Id);
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
    }

}