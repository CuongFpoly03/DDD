using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { error = "Dữ liệu người dùng không hợp lệ." });
                }

                if (string.IsNullOrEmpty(user.UserPassword))
                {
                    return BadRequest(new { error = "Mật khẩu không được để trống." });
                }

                await _authRepository.Register(user, user.UserPassword);

                return Ok(new { message = "Đăng ký thành công!", data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Đăng ký không thành công!", ex });
            }
        }
        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            try
            {
                if (loginUser.UserEmail == null || loginUser.UserPassword == null)
                {
                    return BadRequest(new { error = "Email and password are required" });
                }
                var user = await _authRepository.Login(loginUser.UserEmail, loginUser.UserPassword);
                if (user == null)
                {
                    return Unauthorized(new { error = "Incorrect email or password" });
                }

                var accessToken = _authRepository.GenerateAccessToken(user);
                var refreshToken = _authRepository.GenerateRefreshToken(user);

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new
                {
                    message = "Login successFully !",user,
                    accessToken = accessToken,
                    refreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Làm mới Token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(new { error = "Refresh token not found" });
                }
                var tokens = await _authRepository.RefreshToken(refreshToken);

                Response.Cookies.Append("refreshToken", tokens.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new
                {
                    accessToken = tokens.AccessToken,
                    refreshToken = tokens.RefreshToken
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        // Đăng xuất
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok(new { message = "Logged out successfully!" });
        }
    }
}
