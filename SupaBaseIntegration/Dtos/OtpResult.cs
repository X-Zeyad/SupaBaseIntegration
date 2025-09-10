namespace SupaBaseIntegration.Dtos
{
    public class OtpResult
    {
        public bool Success { get; set; }
        public string? SentTo { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
