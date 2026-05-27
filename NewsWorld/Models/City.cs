using System.ComponentModel.DataAnnotations;

namespace NewsWorld.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required]
        [StringLength(100)]
        public string? CityName { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<News>? News { get; set; }
    }
}