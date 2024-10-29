using Microsoft.EntityFrameworkCore;

namespace Calculator.Models
{
    public class ArticleDetailsViewModel
    {
        public string ArticleName { get; set; }
        public int ArticleId { get; set; }
        public List<ComponentDto> Components { get; set; }
         
        public string  ComponentName { get; set; }

        public int Quantity { get; set; } 
        
        public decimal Price { get; set; }
        public int Id { get; set; }
        public string token { get; set; }
       
    }
}
