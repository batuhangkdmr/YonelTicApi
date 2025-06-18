using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YonelTicApi.Data;
using YonelTicApi.Helpers;


namespace YonelTicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var admins = await _context.Admins.Select(a => new { a.Id, a.Username, a.CreatedAt }).ToListAsync();
            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var admin = await _context.Admins.Select(a => new { a.Id, a.Username, a.CreatedAt }).FirstOrDefaultAsync(a => a.Id == id);
            if (admin == null)
            {
                return NotFound();
            }
            return Ok(admin);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminUpdateRequest request)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            admin.Username = request.Username;
            if (!string.IsNullOrEmpty(request.Password))
            {
                admin.PasswordHash = PasswordHasher.HashPassword(request.Password);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Admin başarıyla güncellendi." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Admin başarıyla silindi." });
        }

        public class AdminUpdateRequest
        {
            public string Username { get; set; } = string.Empty;
            public string? Password { get; set; }
        }
    }
}