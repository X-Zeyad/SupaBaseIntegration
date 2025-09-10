namespace SupaBaseIntegration.Dtos
{
    public class MagicLinkResult
    {
        public bool Success { get; set; }
        public string? SentTo { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
