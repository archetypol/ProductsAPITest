using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Auth.Models;
using ProductsApi.Auth.Repository;
using ProductsApi.Common.Models;

// Generall a bit overloaded, This shouldnt be a god class for all things user and auth
namespace ProductsApi.Auth.Service
{
    public class UserAuthService
    {
        private readonly IUserRepository _repo;
        private readonly JwtFields _jwtSettings;

        // I could get cute and abstract away the authing mechanism here
        // but that would be premature
        public UserAuthService(
            IUserRepository repo,
            IOptions<JwtFields> jwtSettingsAccessor
        )
        {
            _repo = repo;
            _jwtSettings = jwtSettingsAccessor.Value;
        }

        public async Task<Result<ProductsApiUser>> RegisterUser(RegisterRequest model)
        {
            var registerUserResult = await _repo.CreateNewUser(model.Email, model.Password, model.IsAdmin);
            if (!registerUserResult.IsSuccess)
                return Result<ProductsApiUser>.Failure(registerUserResult.Errors);

            return Result<ProductsApiUser>.Success(registerUserResult.Value);
        }

        public async Task<Result<ProductsApiUser>> LoginUser(LoginRequest model)
        {
            var validUserResult = await _repo.CheckUserCredentials(model.Email, model.Password);
            if (!validUserResult.IsSuccess)
                return Result<ProductsApiUser>.Failure(validUserResult.Errors);

            return Result<ProductsApiUser>.Success(validUserResult.Value);
        }

        // This is too tightly coupled currently, its tied to an instance
        // and is reliant on a concrete token type, chuck it on the backlog
        public string MintJWTToken(ProductsApiUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("IsAdmin", user.IsAdmin.ToString().ToLower()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(jwtToken);
        }
    }
}
