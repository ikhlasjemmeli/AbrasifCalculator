using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
    public class ArticleComponent
    {
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        public int ComponentId { get; set; }
        public Component Component { get; set; }

        public int Quantity { get; set; } // Example of additional data in the join table
        [Precision(16, 3)]
        public decimal Price { get; set; }
        public int tva { get; set; }
        [Precision(16, 3)]
        public decimal totalpriceHT { get; set; }
        [Precision(16, 3)]
        public decimal totalpriceTTC { get; set; }
     


    }
}
