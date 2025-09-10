using System.ComponentModel.DataAnnotations;

namespace SupaBaseIntegration.Dtos
{
    public class SignUpDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
    }
}
