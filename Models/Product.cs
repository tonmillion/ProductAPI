
using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; 

namespace ProductApi.Models
{
    public class Product
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int ProductId { get; set; } 

        [Required] 
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty; 

        public string? Description { get; set; } 

        [Column(TypeName = "decimal(10, 2)")] 
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? DiscountPrice { get; set; } 

        public int Stock { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(255)]
        public string? Image { get; set; }

        public DateTime CreatedAt { get; set; } 
        public bool IsActive { get; set; } = true; 
    }
}