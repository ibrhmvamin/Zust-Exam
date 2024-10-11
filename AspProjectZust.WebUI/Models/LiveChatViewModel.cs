using AspProjectZust.Entities.Entity;

namespace AspProjectZust.WebUI.Models
{
    public class LiveChatViewModel
    {
        public string? ReceiverId { get; set; }
        public string? SenderId { get; set; }
        public CustomIdentityUser? Friend { get; set; }
    }
}
