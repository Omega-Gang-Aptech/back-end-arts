using back_end_arts.DTO.User.Response;
using back_end_arts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace back_end_arts.Interface
{
    public interface IJwtService
    {
        Task<LoginResponse> GenerateJwtToken(User user);
        Task<dynamic> VerifyAndGenerateToken(string token, string refreshToken);
    }
}
