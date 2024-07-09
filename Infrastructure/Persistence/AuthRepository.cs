using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Repositories;
using DomainDrivenDesign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DomainDrivenDesign.Infrastructure.Persistence
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        // Danh sách lưu trữ các refresh token
        private static readonly List<string> refreshTokens = new List<string>(); 

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tìm user bằng email
        public async Task<User?> FindByEmail(string? email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == email);
        }

        // Đăng ký user mới
        public async Task Register(User user, string password)
        {
            try
            {
                // Kiểm tra xem email đã tồn tại trong hệ thống chưa
                var existingUser = await FindByEmail(user.UserEmail);
                
                if (existingUser != null)
                {
                    throw new ApplicationException("Email này đã tồn tại !.");
                }

                // Mã hóa mật khẩu trước khi lưu vào database
                user.UserPassword = BCrypt.Net.BCrypt.HashPassword(password);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Register failed!", ex);
            }
        }

        // Đăng nhập
        public async Task<User?> Login(string email, string password)
        {
            // Tìm user bằng email và kiểm tra mật khẩu
            var user = await FindByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.UserPassword))
            {
                return null; // Trả về null nếu thông tin đăng nhập không hợp lệ
            }
            return user; // Trả về user nếu đăng nhập thành công
        }

        // Tạo Access Token từ user
        public string GenerateAccessToken(User user)
        {
            // Tạo JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtAccessKey = Environment.GetEnvironmentVariable("JWT_ACCESS_KEY");
            if (string.IsNullOrEmpty(jwtAccessKey)  || jwtAccessKey.Length < 32)
            {
                throw new InvalidOperationException("JWTaccesskey environment không hợp lệ.");
            }
            var key = Encoding.ASCII.GetBytes(jwtAccessKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.UserId.ToString()), // Thêm claim id của user
                    new Claim("isAdmin", user.IsAdmin.ToString()) // Thêm claim quyền admin của user
                }),
                Expires = DateTime.UtcNow.AddDays(15), // Token hết hạn sau 15 ngày
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); // Trả về string token đã được mã hóa
        }

        // Tạo Refresh Token từ user
        public string GenerateRefreshToken(User user)
        {
            Console.WriteLine("GenerateRefreshToken: " + user.UserId);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtFreshKey = Environment.GetEnvironmentVariable("JWT_REFRESH_KEY");
            if (string.IsNullOrEmpty(jwtFreshKey))
            {
                throw new InvalidOperationException("JWT refresh key environment không hợp lệ.");
            }
            var key = Encoding.ASCII.GetBytes(jwtFreshKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.UserId.ToString()), // Thêm claim id của user
                    new Claim("isAdmin", user.IsAdmin.ToString()) // Thêm claim quyền admin của user
                }),
                Expires = DateTime.UtcNow.AddDays(365), // Token hết hạn sau 1 năm
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = tokenHandler.WriteToken(token); // Tạo string refresh token đã mã hóa
            refreshTokens.Add(refreshToken); // Thêm refreshToken vào danh sách refreshTokens
            return refreshToken; // Trả về refreshToken
        }

        // Làm mới AccessToken từ RefreshToken
        public async Task<(string AccessToken, string RefreshToken)> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken) || !refreshTokens.Contains(refreshToken))
            {
                throw new ApplicationException("Refresh token is not valid");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtFreshKey = Environment.GetEnvironmentVariable("JWT_REFRESH_KEY");
            if (string.IsNullOrEmpty(jwtFreshKey))
            {
                throw new InvalidOperationException("JWT refresh key environment Không hợp lệ.");
            }
            var key = Encoding.ASCII.GetBytes(jwtFreshKey);

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out validatedToken);

            var userId = principal.FindFirst("id")?.Value;
            if(userId == null){
                throw new ApplicationException("Refresh token is not valid");
            }
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                throw new ApplicationException("Refresh token is not valid");
            }

            refreshTokens.Remove(refreshToken); // Xóa refreshToken khỏi danh sách
            var newAccessToken = GenerateAccessToken(user); // Tạo AccessToken mới
            var newRefreshToken = GenerateRefreshToken(user); // Tạo RefreshToken mới
            refreshTokens.Add(newRefreshToken); // Thêm refreshToken mới vào danh sách

            return (newAccessToken, newRefreshToken); // Trả về cặp AccessToken và RefreshToken mới
        }
    }
}
