using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using OweMeApi.Data;
using OweMeApi.Data.Entities;
using OweMeApi.Dtos.Users;
using OweMeApi.Services;

namespace OweMeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(AppDbContext context, AuthService authService) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly AuthService _authService = authService;

        [HttpPost("sign-up")]
        public async Task<ActionResult<string>> SignUp(UserSignUpDTO request)
        {
            if(await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("This email is already in use.");
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            User user = new()
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                FullName = request.FullName,
                Hash = hash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<string>> SignIn(UserSignInDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Hash))
                return BadRequest("Wrong email or password");

            var token = _authService.CreateToken(user);
            return Ok(token);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users =  await _context.Users
                .Select(u => new UserDTO(u.Id, u.Email, u.FullName))
                .ToListAsync();

            return Ok(users);
        }
    }
}
