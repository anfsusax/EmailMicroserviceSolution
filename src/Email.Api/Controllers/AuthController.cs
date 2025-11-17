using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Email.Api.Configuration;
using Email.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Email.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtOptions _options;

    public AuthController(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public IActionResult GenerateToken([FromBody] GenerateTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return BadRequest("UserName é obrigatório.");
        }

        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret.PadRight(32, '0')));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(1);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.UserName),
            new(JwtRegisteredClaimNames.UniqueName, request.UserName),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, request.Email));
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, request.Role));
        }

        var token = handler.WriteToken(new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials));

        return Ok(new TokenResponse(token, expires));
    }
}

