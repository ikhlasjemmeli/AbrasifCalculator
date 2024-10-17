namespace Calculator.Models
{
    public class TvaUpdateModelDto
    {
        public int componentId { get; set; }
        public int articleId { get; set; }
        public int Tva { get; set; }
        public decimal totalpriceTTC { get; set; }
    }
}
