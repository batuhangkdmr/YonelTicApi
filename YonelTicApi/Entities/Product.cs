using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YonelTicApi.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public string? CloudinaryPublicId { get; set; }

        // Category relationship
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        // SubCategory relationship
        public int? SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public Category? SubCategory { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
