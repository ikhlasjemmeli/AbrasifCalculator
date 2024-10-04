using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
    public class ComponentDto

    {
        public List<Article> articles { get; set; }
        public string artName { get; set; } = "";
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = "";
        [Required]
        public int Quantity { get; set; }
        [Required, Precision(16, 3)]
        public decimal Price { get; set; }
    }
}
