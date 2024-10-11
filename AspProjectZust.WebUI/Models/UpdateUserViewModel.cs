using System.ComponentModel.DataAnnotations;

namespace AspProjectZust.WebUI.Models
{
    public class UpdateUserViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? BackUpEmail { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Occupation { get; set; }
        public int Gender { get; set; }
        public int RelationStatus { get; set; }
        public int BloodGroup { get; set; }
        public int Language { get; set; }
        public string? Address { get; set; }
        public int Country { get; set; }
        public int City { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? ChangePassword { get; set; }
    }
}
