using ApiManagerStudent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiManagerStudent.Services
{
    public interface IJwtService
    {
        Task<AuthResponse> GetTokenAsync(AuthRequest authRequest,string ipAddress);
        Task<AuthResponse> GetRefreshTokenAsync(string ipAddress, int userId, int id);
        Task<bool> IsTokenValid(string accessToken, string ipAddress);
    }
}
