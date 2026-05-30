using Assessment.Models;

namespace Assessment.Services
{
    public class ReviewService
    {
        private readonly List<Review> _reviews = new();
        private int _nextId = 1;

        public List<Review> GetReviewsByDishId(int dishId)
        {
            return _reviews.Where(r => r.DishId == dishId).OrderByDescending(r => r.ReviewDate).ToList();
        }

        public List<Review> GetAllReviews()
        {
            return _reviews.OrderByDescending(r => r.ReviewDate).ToList();
        }

        public void AddReview(Review review)
        {
            review.Id = _nextId++;
            review.ReviewDate = DateTime.Now;
            _reviews.Add(review);
        }
    }
}