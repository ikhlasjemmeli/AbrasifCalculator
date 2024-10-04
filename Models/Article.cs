﻿namespace Calculator.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<ArticleComponent> ArticleComponents { get; set; } = new List<ArticleComponent>();

    }
}
