using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly APIContext _context;

    public UsersController(APIContext context)
    {
        _context = context;
    }
       
    // Método para criar uma nova conta
    [HttpPost("/createUser")]
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

    // Método para adicionar um amigo
    [Authorize]
    [HttpPost("/addFriend")]
    public async Task<IActionResult> AddFriend(string identifier)
    {
        // Obter o email do usuário autenticado a partir do token JWT
        var userEmail = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (userEmail == null) return Unauthorized("Usuário não autenticado.");

        // Buscar o usuário no banco de dados usando o email
        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return Unauthorized("Usuário não encontrado.");

        if (identifier == null) return BadRequest("Entre com um valor valido");
        var existUser = await _context.User.FirstOrDefaultAsync(m =>
            m.Name == identifier ||
            m.NickName == identifier ||
            m.Email == identifier
        );

        if (existUser == null) return NotFound("User não existe");        

        user.Friends.Add(existUser);
        await _context.SaveChangesAsync();
        return Ok("Amigo adicionado com sucesso");
    }

    // Método GET para obter as reviews do usuário
    [HttpGet("{userId}/UserReviews")]
    public async Task<ActionResult<IEnumerable<Review>>> GetUserReviews(string userId)
    {
        // Verifica se o user existe
        var user = await _context.User.FirstOrDefaultAsync(m => m.Name == userId || m.NickName == userId || m.Email == userId);
        if (user == null) return NotFound($"User {userId} not found.");

        // Obtém todas as reviews do user
        var reviews = await _context.Review
            .Where(r => r.UserId == user.Id)
            .ToListAsync();

        return Ok(reviews);
    }

    // Método GET para retornar os amigos de um usuário específico
    [HttpGet("{userId}/Friends")]
    public async Task<IActionResult> GetUserFriends(string userId)
    {
        var user = await _context.User
            .Include(u => u.Friends)  // Incluir os amigos do usuário
            .FirstOrDefaultAsync(u => u.Name == userId || u.Email == userId || u.NickName == userId);

        if (user == null) return NotFound($"User {userId}not found");

        List<User> friends = new List<User>();

        foreach (var f in user.Friends)
        {
            User friend = new User
            {
                Id = f.Id,
                Name = f.Name,
                NickName = f.NickName,
                Email = f.Email
            };
            friends.Add(friend);
        }
        return Ok(friends);
    }

    [HttpGet("searchUser")]
    public async Task<IActionResult> SearchMovie(string query)
    {
        if (string.IsNullOrEmpty(query)) return BadRequest("Query null");

        query = query.ToLower();

        var users = await _context.User
            .Where(u => u.Name.ToLower()
            .Contains(query))
            .Select(u => new UserDTO {
                Name = u.Name,
                Email = u.Email, 
                Nickname = u.NickName
            })
            .ToListAsync();

        return Ok(users);
    }

    // Método GET para obter informações gerais do usuário
    [HttpGet("{userId}/UserData")]
    public async Task<IActionResult> GetUserData(string userId)
    {
        var user = await _context.User.FirstOrDefaultAsync(m => m.Name == userId || m.Email == userId || m.NickName == userId);
        if (user == null) return NotFound($"User {userId} not found.");
        UserDTO existUser = new UserDTO
        {
            Name = user.Name,
            Email = user.Email,
            Nickname = user.NickName
        };
        return Ok(existUser);
    }
}
