using ApiManagerStudent.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using ApiManagerStudent.EF;
using ApiManagerStudent.Support;
using Microsoft.EntityFrameworkCore;

namespace ApiManagerStudent.Services
{
    public class JwtService : IJwtService
    {
        private readonly ManagerStudentContext _context;
        private readonly IConfiguration _configuration;

        public JwtService(ManagerStudentContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> GetRefreshTokenAsync(string ipAddress, int userId, int id)
        {
            var refreshToken = GenerateRefreshToken();
            var teacher = await _context.Teachers.FindAsync(id);
            var accessToken = GenerateToken(teacher);
            return await SaveTokenDetails(ipAddress, userId, accessToken, refreshToken);
        }

        public async Task<AuthResponse> GetTokenAsync(AuthRequest authRequest, string ipAddress)
        {
            authRequest.Password = Libary.Instances.EncodeMD5(authRequest.Password);
            var user = _context.Teachers.FirstOrDefault(x => x.Username.Equals(authRequest.UserName)
            && x.Password.Equals(authRequest.Password));
            if (user == null)
                return await Task.FromResult<AuthResponse>(null);
            string tokenString = GenerateToken(user);
            string refreshToken = GenerateRefreshToken();
            return await SaveTokenDetails(ipAddress, user.Id, tokenString, refreshToken);

        }

        private async Task<AuthResponse> SaveTokenDetails(string ipAddress, int userId, string tokenString, string refreshToken)
        {
            var userRefreshToken = new UserRefreshToken
            {
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(5),
                IpAddress = ipAddress,
                IsInvalidated = false,
                RefreshToken = refreshToken,
                Token = tokenString,
                TeacherId = userId
            };
            await _context.UserRefreshTokens.AddAsync(userRefreshToken);
            await _context.SaveChangesAsync();
            return new AuthResponse { Token = tokenString, RefreshToken = refreshToken, IsSuccess = true };
        }

        private string GenerateRefreshToken()
        {
            var byteArray = new byte[64];
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                cryptoProvider.GetBytes(byteArray);

                return Convert.ToBase64String(byteArray);
            }
        }

        private string GenerateToken(Teacher user)
        {
            var jwtKey = _configuration.GetValue<string>("JwtSettings:Key");
            var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

            var tokenHandler = new JwtSecurityTokenHandler();

            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id+""),
                }),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
               SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(descriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public async Task<bool> IsTokenValid(string accessToken, string ipAddress)
        {
            var isValid = _context.UserRefreshTokens.FirstOrDefault(x => x.Token == accessToken
            && x.IpAddress == ipAddress) != null;
            return await Task.FromResult(isValid);
        }
    }
}
