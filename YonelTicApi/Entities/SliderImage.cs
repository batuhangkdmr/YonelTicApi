using System.ComponentModel.DataAnnotations;

namespace YonelTicApi.Entities
{
    public class SliderImage
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public string CloudinaryPublicId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
