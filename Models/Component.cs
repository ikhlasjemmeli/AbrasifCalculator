using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
    public class Component
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = "";

        public string Unit { get; set; }
        public int UserId { get; set; }
        public ICollection<ArticleComponent> ArticleComponents { get; set; } = new List<ArticleComponent>();


    }
}
