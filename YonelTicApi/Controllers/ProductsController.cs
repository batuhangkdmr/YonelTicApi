using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YonelTicApi.Data;
using YonelTicApi.Entities;
using YonelTicApi.Services;

namespace YonelTicApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly CloudinaryService _cloudinaryService;

        public ProductsController(ApplicationDbContext context, CloudinaryService cloudinaryService)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // GET: api/products
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts(
            int page = 1,
            int pageSize = 30,
            string? categoryId = null,
            string? subCategory = null,
            string? search = null)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(categoryId) && int.TryParse(categoryId, out int catId))
            {
                query = query.Where(p => p.CategoryId == catId);
            }

            if (!string.IsNullOrEmpty(subCategory) && subCategory != "all")
            {
                query = query.Where(p => p.SubCategory.Name == subCategory);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                products,
                totalPages,
                totalProducts
            });
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        public class ProductCreateDto
        {
            public string name { get; set; }
            public string description { get; set; }
            public int? categoryId { get; set; }
            public int? subCategoryId { get; set; }
            public string? cloudinaryPublicId { get; set; }
            public IFormFile? image { get; set; }
        }

        public class ProductUpdateDto
        {
            public string name { get; set; }
            public string description { get; set; }
            public int? categoryId { get; set; }
            public int? subCategoryId { get; set; }
            public string? cloudinaryPublicId { get; set; }
            public IFormFile? image { get; set; }
        }

        // POST: api/products
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = new Product
            {
                Name = dto.name,
                Description = dto.description,
                CategoryId = dto.categoryId,
                SubCategoryId = dto.subCategoryId
            };

            if (dto.image != null && dto.image.Length > 0)
            {
                var (url, publicId) = await _cloudinaryService.UploadImageAsync(dto.image);
                product.ImageUrl = url;
                product.CloudinaryPublicId = publicId;
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto dto)
        {
            // image alanı gelmediyse ModelState'den hatasını temizle
            if (ModelState.ContainsKey("image"))
                ModelState["image"].Errors.Clear();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = dto.name;
            product.Description = dto.description;
            product.CategoryId = dto.categoryId;
            product.SubCategoryId = dto.subCategoryId;

            if (!string.IsNullOrEmpty(dto.cloudinaryPublicId))
            {
                product.CloudinaryPublicId = dto.cloudinaryPublicId;
            }

            if (dto.image != null && dto.image.Length > 0)
            {
                await _cloudinaryService.DeleteImageAsync(product.CloudinaryPublicId);
                var (url, publicId) = await _cloudinaryService.UploadImageAsync(dto.image);
                product.ImageUrl = url;
                product.CloudinaryPublicId = publicId;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await _cloudinaryService.DeleteImageAsync(product.CloudinaryPublicId);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}