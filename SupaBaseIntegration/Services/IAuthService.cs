using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using SupaBaseIntegration.Dtos;
using OAuthOptions = SupaBaseIntegration.Dtos.OAuthOptions;

namespace SupaBaseIntegration.Services
{
    public interface IAuthService
    {
        Task<AuthResult> SignUpAsync(SignUpDto dto);
        Task<AuthResult> SignInAsync(string email, string password);
        Task<bool> SignOutAsync(string? token = null , string? refreshToken= null);
        Task<string?> GetCurrentUserAsync(string? token = null);

        // Magic Link & OTP
        Task<MagicLinkResult> SendMagicLinkAsync(string email, string? redirectTo = null);
        Task<OtpResult> SendOtpAsync(string phone);
        Task<AuthResult> VerifyOtpAsync(string token, string? email = null, string? phone = null);

        // OAuth
        Task<OAuthUrlResult> GetOAuthUrlAsync(string provider, OAuthOptions? options = null);
        Task<AuthResult> HandleOAuthCallbackAsync(string accessToken, string? refreshToken = null);

        // Session Management
        Task<bool> RefreshSessionAsync(string refreshToken);
        Task<bool> IsTokenValidAsync(string token);
        Task<User?> GetUserFromTokenAsync(string token);
    }
}
