using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        // #10. Adding validation
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}