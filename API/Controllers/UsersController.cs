using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly APIContext _context;

    public UsersController(APIContext context)
    {
        _context = context;
    }
        
    [HttpPost("/addUser")]
    public async Task<IActionResult> CreateUser(UserRequest userRequest)
    {
        if (userRequest == null) return BadRequest("User não pode ser nulo!");

        var existUser = await _context.User.FirstOrDefaultAsync(u => u.Email == userRequest.Email);

        if (existUser == null) // Usuario não existe no banco de dados
        {
            User obj = new User { 
                Name = userRequest.Name, 
                Email = userRequest.Email, 
                NickName = userRequest.NickName, 
                Password = userRequest.Password 
            };
            _context.User.Add(obj);
            await _context.SaveChangesAsync();
            return Ok(obj);
        }
        return BadRequest("Já existe usuario com esse email");
    }

    [HttpGet("getUserReviews/{userName}")]
    public async Task<ActionResult<IEnumerable<Review>>> GetUserReviews(string userName)
    {
        // Verifica se o user existe
        var user = await _context.User.FirstOrDefaultAsync(m => m.Name == userName);
        if (user == null) return NotFound($"User with name {userName} not found.");

        // Obtém todas as reviews do user
        var reviews = await _context.Review
            .Where(r => r.UserId == user.Id)
            .ToListAsync();

        return Ok(reviews);
    }

}
