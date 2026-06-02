namespace Assessment.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int DishId { get; set; }

        public string DishName { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public int Rating { get; set; }

        public string? PhotoPath { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
}
