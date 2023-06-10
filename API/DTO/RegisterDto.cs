using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 4)]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string KnownAs { get; set; }

        [Required]
        public DateOnly? DateOfBirth { get; set; } // optional to make required work, instead of default date

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }
    }
}