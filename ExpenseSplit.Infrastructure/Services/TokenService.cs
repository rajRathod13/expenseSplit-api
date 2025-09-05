using ExpenseSplit.Application.ServiceInterfaces;
using ExpenseSplit.Common.ConfigurationSettings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseSplit.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    public string GenerateToken(string email, string userId, List<string> roles)
    {
        var now = DateTime.UtcNow;
        //if (roles is not null && roles.Count > 0)
        //    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim("age", "22"),
            new Claim("userId", userId),
        };

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var securityToken = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            NotBefore = now,
            IssuedAt = now,
            Expires = now.AddMinutes(_jwtSettings.TokenExpirationMinutes),
            SigningCredentials = credentials
        };

        //var securityToken = new JwtSecurityToken(
        //    issuer:_jwtSettings.Issuer,
        //    audience:_jwtSettings.Audience,
        //    claims:identity.Claims,
        //    expires:DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationMinutes),
        //    signingCredentials:credentials
        //    );

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(securityToken);
        var jwt = handler.WriteToken(token);
        return jwt;
    }

    //public string GetEmailFromToken(string token)
    //{
    //    var handler = new JwtSecurityTokenHandler();
    //    handler.InboundClaimTypeMap.Clear();

    //    //var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
    //    var validationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuer = false,
    //        ValidateAudience = false,
    //        ValidateLifetime = true,
    //        ValidIssuer = _jwtSettings.Issuer,
    //        ValidAudience = _jwtSettings.Audience,
    //        ValidateIssuerSigningKey =true,
    //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
    //        ClockSkew = TimeSpan.Zero
    //    };

    //    try
    //    {
    //        //var temp = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
    //        //var temp1 = jwtToken?.Claims.FirstOrDefault(c => c.Type == "age");
    //        var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
    //        var emailClaim = principal?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email || c.Type == "email");
    //        var ageClaim = principal?.Claims.FirstOrDefault(c => c.Type == "age");
    //        return emailClaim?.Value;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("Token validation failed: " + ex.Message);
    //        return null;
    //    }
    //}
}
