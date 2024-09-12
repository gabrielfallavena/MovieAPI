using API.Data;
using API.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly APIContext _context;

    public AuthController(APIContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null) return BadRequest("Entre com dados válidos");

        var existUser = await _context.User.FirstOrDefaultAsync(p => p.Email == loginRequest.Email && p.Password == loginRequest.Password);
        
        if (existUser == null) return Unauthorized("User não existe no banco de dados");

        if (existUser.Password != loginRequest.Password) return Unauthorized("Senha incorreta");

        // Se o login for bem-sucedido, gerar o token JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("b3d5E1fFgG7hRkJjD9sP4kQwMnXyZt9JcHwUdGsF1v8YsErA5T3B");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, existUser.Email),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}

