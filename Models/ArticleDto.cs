using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
    public class ArticleDto
    {
    
        public int Id { get; set; }
        public string Nameart { get; set; } = "";
        public ComponentDto ComponentDto { get; set; } 

    }
}
