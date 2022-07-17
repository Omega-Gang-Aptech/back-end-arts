using back_end_arts.DTO.User.Response;
using back_end_arts.Interface;
using back_end_arts.Models;
using back_end_arts.Repository;
using back_end_arts.Specification;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace back_end_arts.Service
{
    public class JwtService : IJwtService
    {
        //doc ket noi
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly ArtsDbWithKeyContext _context;
        private readonly IArtsRepository<RefreshToken> _refreshTokenRepository;
        private readonly IArtsRepository<User> _employeeRepository;
        private readonly IArtsRepository<Role> _roleRepository;
        public JwtService(
               ArtsDbWithKeyContext context,
               IArtsRepository<RefreshToken> refreshTokenRepository,
               IArtsRepository<User> userRepository,
               IArtsRepository<Role> roleRepository,
               TokenValidationParameters tokenValidationParams,
               IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            this._refreshTokenRepository = refreshTokenRepository;
            //ket noi tu dong
            this._jwtConfig = optionsMonitor.CurrentValue;
            this._tokenValidationParams = tokenValidationParams;
            this._employeeRepository = userRepository;
            this._roleRepository = roleRepository;
            this._context = context;
        }

        //cap jwt
        public async Task<LoginResponse> GenerateJwtToken(User user)
        {
            var utcNow = DateTime.UtcNow;
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    //new Claim("Id", user.UserId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.UserEmail),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                }),
                Expires = utcNow.AddHours(_jwtConfig.ExpiredInHours), // 5-10 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            };

            //user.UserRoles.ToList().ForEach(u => { tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, u.Role.Name)); });

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);




            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.UserId,
                AddedDate = utcNow,
                ExpiryDate = utcNow.AddHours(_jwtConfig.ExpiredInHours),
                Token = RandomString(35) + Guid.NewGuid()
            };

            await _refreshTokenRepository.Insert(refreshToken);

            // lay role cua employee, gan user Role ben duoi
            //var roles = await _userManager.GetRolesAsync(user); new List<string>()
            var check_data = (from a in this._context.Roles
                              where a.RoleId.Equals(user.UserRole)
                              select a.RoleName);

            if (check_data != null)
            {
                return new LoginResponse
                {
                    UserName = user.UserName,
                    Token = jwtToken,
                    RequestToken = refreshToken.Token,
                    UserRoles = check_data,
                };
            }
            else
            {
                return new LoginResponse
                {
                    UserName = user.UserName,
                    Token = jwtToken,
                    RequestToken = refreshToken.Token,
                    UserRoles = null,
                };
            }


        }

        public async Task<dynamic> VerifyAndGenerateToken(string token, string refreshToken)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();


            try
            {
                // Validation 1 - Validation JWT token format
                // var tokenInVerification = jwtTokenHandler.ValidateToken(token, _tokenValidationParams, out var validatedToken);
                var validatedToken = jwtTokenHandler.ReadJwtToken(token);

                // Validation 2 - Validate encryption alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false)
                    {
                        return null;
                    }
                }

                // Validation 3 - validate expiry date
                var utcExpiryDate = long.Parse(validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has not yet expired"
                        }
                    };
                }

                // validation 4 - validate existence of the token
                var tokenSpec = new RefreshTokenSpec(refreshToken);
                var storedToken = await _refreshTokenRepository.GetAsyncSpec(tokenSpec);
                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    storedToken.IsRevoked = true;

                    await _refreshTokenRepository.Update(storedToken);

                    return new
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Refresh Token has expired"
                        }
                    };
                }

                if (storedToken == null)
                {
                    return new
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token does not exist"
                        }
                    };
                }

                // Validation 5 - validate if used
                if (storedToken.IsUsed)
                {
                    return new
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been used"
                        }
                    };
                }

                // Validation 6 - validate if revoked
                if (storedToken.IsRevoked)
                {
                    return new
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been revoked"
                        }
                    };
                }
                // Validation 7 - validate the id
                var jti = validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if (storedToken.JwtId != jti)
                {
                    return new
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token doesn't match"
                        }
                    };
                }

                // update current token 

                storedToken.IsUsed = true;

                await _refreshTokenRepository.Update(storedToken);
                // Generate a new token
                var dbUser = await _employeeRepository.GetById(storedToken.UserId); // _userManager.Users.Where(u => u.Id == storedToken.UserId).FirstOrDefault();
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {

                    return new
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has expired please re-login"
                        }
                    };

                }
                else
                {
                    return new
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Something went wrong."
                        }
                    };
                }
            }
        }

        private static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }
    }
}   

