using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YonelTicApi.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Parent category (null if this is a top-level category)
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Category? Parent { get; set; }

        // Child categories
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        // Products in this category
        public ICollection<Product> Products { get; set; } = new List<Product>();

        // Products where this is a subcategory
        public ICollection<Product> SubCategoryProducts { get; set; } = new List<Product>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
