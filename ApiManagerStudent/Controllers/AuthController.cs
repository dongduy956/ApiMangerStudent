using ApiManagerStudent.EF;
using ApiManagerStudent.Models;
using ApiManagerStudent.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiManagerStudent.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableCors("MyPolicy")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ManagerStudentContext _context;

        public AuthController(IJwtService jwtService, ManagerStudentContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Token(AuthRequest authRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { IsSuccess = false, Reason = "UserName and Password must be provided." });
            var authResponse = await _jwtService.GetTokenAsync(authRequest, HttpContext.Connection.RemoteIpAddress.ToString());
            if (authResponse == null)
                return Unauthorized();
            return Ok(authResponse);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { IsSuccess = false, Reason = "Tokens must be provided" });
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var token = GetJwtToken(request.ExpiredToken);
            var userRefreshToken = _context.UserRefreshTokens.FirstOrDefault(
                x => x.IsInvalidated == false && x.Token == request.ExpiredToken
                && x.RefreshToken == request.RefreshToken
                && x.IpAddress == ipAddress);
            userRefreshToken.IsInvalidated = true;
            _context.UserRefreshTokens.Update(userRefreshToken);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthResponse { IsSuccess = false, Reason = "Tokens must be provided" });
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var token = GetJwtToken(request.ExpiredToken);
            var userRefreshToken = _context.UserRefreshTokens.FirstOrDefault(
                x => x.IsInvalidated == false && x.Token == request.ExpiredToken
                && x.RefreshToken == request.RefreshToken
                && x.IpAddress == ipAddress);

            AuthResponse response = ValidateDetails(token, userRefreshToken);
            if (!response.IsSuccess)
                return BadRequest(response);

            userRefreshToken.IsInvalidated = true;
            _context.UserRefreshTokens.Update(userRefreshToken);
            await _context.SaveChangesAsync();

            int id =int.Parse(token.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId).Value);
            var authResponse = await _jwtService.GetRefreshTokenAsync(ipAddress, userRefreshToken.TeacherId,
                id);
            return Ok(authResponse);
        }

        private AuthResponse ValidateDetails(JwtSecurityToken token, UserRefreshToken userRefreshToken)
        {
            if (userRefreshToken == null)
                return new AuthResponse { IsSuccess = false, Reason = "Invalid Token Details." };
            if (token.ValidTo > DateTime.UtcNow)
                return new AuthResponse { IsSuccess = false, Reason = "Token not expired." };
            if (!userRefreshToken.IsActive)

                return new AuthResponse { IsSuccess = false, Reason = "Refresh Token Expired" };
            return new AuthResponse { IsSuccess = true };
        }

        private JwtSecurityToken GetJwtToken(string expiredToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(expiredToken);
        }
    }
}
