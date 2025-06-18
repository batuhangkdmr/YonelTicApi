using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YonelTicApi.Data;
using YonelTicApi.Entities;
using YonelTicApi.Helpers;

namespace YonelTicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _jwtTokenHelper = new JwtTokenHelper(configuration);
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string PasswordRepeat { get; set; } = string.Empty;
            public string SecretKey { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == request.Username);
            if (admin == null || !PasswordHasher.VerifyPassword(request.Password, admin.PasswordHash))
            {
                return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı." });
            }

            var token = _jwtTokenHelper.GenerateToken(admin);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request.SecretKey != _configuration["AdminSecretKey"])
            {
                return BadRequest(new { message = "Geçersiz secret key." });
            }

            if (request.Password != request.PasswordRepeat)
            {
                return BadRequest(new { message = "Şifreler aynı olmalı." });
            }

            if (await _context.Admins.AnyAsync(a => a.Username == request.Username))
            {
                return BadRequest(new { message = "Bu kullanıcı adı zaten kullanılıyor." });
            }

            var admin = new Admin
            {
                Username = request.Username,
                PasswordHash = PasswordHasher.HashPassword(request.Password)
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin başarıyla kaydedildi." });
        }
    }
}