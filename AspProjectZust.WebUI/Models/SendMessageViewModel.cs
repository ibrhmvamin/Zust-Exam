using AspProjectZust.Entities.Entity;

namespace AspProjectZust.WebUI.Models
{
    public class SendMessageViewModel
    {
        public string? CurrentUserId { get; set; }
        public Chat? Chat { get; set; }
        public string? ReceIverImageUrl { get; set; }
        public string? SenderImageUrl { get; set; }
        public string? ReceiverName { get; set; }
        public string? SenderName { get; set; }
    }
}