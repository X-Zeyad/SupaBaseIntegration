namespace SupaBaseIntegration.Dtos
{
    public class OAuthOptions
    {
        public string? RedirectTo { get; set; }
        public List<string>? Scopes { get; set; }
        public Dictionary<string, string>? QueryParams { get; set; }
    }
}
