﻿namespace Calculator.Models
{
    public class ListComponentDto
    {
        public List<Component> Components { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string token { get; set; }
    }
}
