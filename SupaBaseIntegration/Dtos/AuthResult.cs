using Supabase.Gotrue;

namespace SupaBaseIntegration.Dtos
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public User? User { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
