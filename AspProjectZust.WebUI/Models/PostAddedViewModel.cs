using AspProjectZust.Entities.Entity;

namespace AspProjectZust.WebUI.Models
{
    public class PostAddedViewModel
    {
        public string? Content { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? Video { get; set; }
        public string? VideoUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? TagFriends { get; set; }

        public string? Email { get; set; }
        public string? UserName { get; set; }
        public IFormFile? File { get; set; }
        public int userRequestCount { get; set; }
    }
}
