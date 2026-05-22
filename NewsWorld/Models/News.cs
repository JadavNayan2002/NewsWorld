using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsWorld.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? SortDescription { get; set; }

        public string? FullDescription { get; set; }

        public string? ImagePath { get; set; }

        public string? VideoUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Foreign Key
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}