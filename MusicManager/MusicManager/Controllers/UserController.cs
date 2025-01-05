using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Services;

namespace MusicManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            if (!user.Active)
            {
                res.code = 402;
                res.message = "Tài khoản của bạn chưa được kích hoạt!";
                return Ok(res);
            }
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var accessToken = await _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                // Lưu refresh token trong bộ nhớ
                RefreshTokens[user.UserName] = refreshToken;
                res.data = new ResponseToken { AccessToken = accessToken, RefreshToken = refreshToken };
                return Ok(res);
            }
            res.code = 401;
            res.message = "Tài khoản đăng nhập không đúng!";
            return Ok(res);
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var username = principal.Identity.Name;

            if (!RefreshTokens.TryGetValue(username, out var savedRefreshToken) || savedRefreshToken != request.RefreshToken)
            {
                return Unauthorized();
            }

            var newAccessToken = _tokenService.GenerateAccessToken(new ApplicationUser { UserName = username }).Result;
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Cập nhật refresh token
            RefreshTokens[username] = newRefreshToken;
            var res = new ResponseData<ResponseToken>()
            {
                data = new ResponseToken { AccessToken = newAccessToken, RefreshToken = newRefreshToken }
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
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                IsAdmin = model.IsAdmin,
                ArtistName = model.ArtistName,
                RevenuePercentage = model.RevenuePercentage,
                Name= model.FullName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
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
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest model)
        {
            var res = new ResponseBase();
            var user = await _userManager.FindByIdAsync(id);
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
        [HttpDelete("delete/{id}")]
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
    }

}