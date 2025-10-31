using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniProjectApi.Data;
using MiniProjectApi.Dtos;
using MiniProjectApi.Models;
using MiniProjectApi.Services;

namespace MiniProjectApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This makes the route /api/auth
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly TokenService _tokenService;

        public AuthController(DataContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest("Email is already taken.");
            }

            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = passwordHash
            };

            // Save user to database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create a token
            var token = _tokenService.CreateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email
            });
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            // Find the user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // Check if user exists and password is correct
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Create a token
            var token = _tokenService.CreateToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = user.Email
            });
        }
    }
}