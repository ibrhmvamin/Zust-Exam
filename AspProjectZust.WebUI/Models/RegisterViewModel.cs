using System.ComponentModel.DataAnnotations;

namespace AspProjectZust.WebUI.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        public bool IsAcceptThePrivacy { get; set; }
    }
}
