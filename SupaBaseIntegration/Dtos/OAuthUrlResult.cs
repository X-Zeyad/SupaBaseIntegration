namespace SupaBaseIntegration.Dtos
{
    public class OAuthUrlResult
    {
        public bool Success { get; set; }
        public string? Provider { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
