using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        // #10. Adding validation
            // สามารถใช้ได้แม้กระทั้ง regular expression
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}