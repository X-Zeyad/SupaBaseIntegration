namespace SupaBaseIntegration.Dtos
{
    public class ProviderInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool RequiresScopes { get; set; }
        public string[] DefaultScopes { get; set; } = Array.Empty<string>();
    }
}
