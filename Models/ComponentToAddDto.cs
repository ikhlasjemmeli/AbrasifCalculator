using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
    public class ComponentToAddDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        [Required, Precision(16, 3)]
        public decimal Price { get; set; }
    }
}
