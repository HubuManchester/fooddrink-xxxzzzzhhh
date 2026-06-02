namespace Assessment.Models
{
    public class Dish
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImageName { get; set; } = string.Empty;

        public double Rating { get; set; }

        public bool IsSpicy { get; set; }

        public bool IsVegetarian { get; set; }

        public int PrepTimeMinutes { get; set; }
    }
}
