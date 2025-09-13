using System.ComponentModel.DataAnnotations;

namespace SupaBaseIntegration.Dtos.Vdocipher
{
    public class OtpRequestDto
    {
        [Required]
        public string VideoId { get; set; }
        public int Ttl { get; set; }=300;
    }
}
