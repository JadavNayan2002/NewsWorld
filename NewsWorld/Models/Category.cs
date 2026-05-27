using System.ComponentModel.DataAnnotations;

namespace NewsWorld.Models
{
    public class Category
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        public string? CategoryName { get; set; }
        public ICollection<News>? News { get; set; }
    }
}