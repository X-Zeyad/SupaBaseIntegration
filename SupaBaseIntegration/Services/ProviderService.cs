using SupaBaseIntegration.Dtos;
using static Supabase.Gotrue.Constants;

namespace SupaBaseIntegration.Services
{
    public class ProviderService : IProviderService
    {
        private readonly ILogger<ProviderService> _logger;

        private static readonly Dictionary<Provider, ProviderInfo> _supportedProviders = new()
        {
            [Provider.Github] = new ProviderInfo
            {
                Name = "github",
                DisplayName = "GitHub",
                IconUrl = "https://github.com/favicon.ico",
                Description = "Sign in with your GitHub account",
                RequiresScopes = true,
                DefaultScopes = new[] { "user:email", "read:user" }
            },
            [Provider.Google] = new ProviderInfo
            {
                Name = "google",
                DisplayName = "Google",
                IconUrl = "https://www.google.com/favicon.ico",
                Description = "Sign in with your Google account",
                RequiresScopes = true,
                DefaultScopes = new[] { "openid", "email", "profile" }
            },
            [Provider.Discord] = new ProviderInfo
            {
                Name = "discord",
                DisplayName = "Discord",
                IconUrl = "https://discord.com/assets/favicon.ico",
                Description = "Sign in with your Discord account",
                RequiresScopes = true,
                DefaultScopes = new[] { "identify", "email" }
            },
            [Provider.Twitter] = new ProviderInfo
            {
                Name = "twitter",
                DisplayName = "Twitter",
                IconUrl = "https://twitter.com/favicon.ico",
                Description = "Sign in with your Twitter account",
                RequiresScopes = false,
                DefaultScopes = Array.Empty<string>()
            },
            [Provider.Facebook] = new ProviderInfo
            {
                Name = "facebook",
                DisplayName = "Facebook",
                IconUrl = "https://www.facebook.com/favicon.ico",
                Description = "Sign in with your Facebook account",
                RequiresScopes = true,
                DefaultScopes = new[] { "email", "public_profile" }
            },
            [Provider.Apple] = new ProviderInfo
            {
                Name = "apple",
                DisplayName = "Apple",
                IconUrl = "https://www.apple.com/favicon.ico",
                Description = "Sign in with your Apple ID",
                RequiresScopes = true,
                DefaultScopes = new[] { "name", "email" }
            },
            [Provider.Azure] = new ProviderInfo
            {
                Name = "azure",
                DisplayName = "Microsoft",
                IconUrl = "https://www.microsoft.com/favicon.iconv2", // not working
                Description = "Sign in with your Microsoft account",
                RequiresScopes = true,
                DefaultScopes = new[] { "openid", "profile", "email" }
            },
            [Provider.LinkedIn] = new ProviderInfo
            {
                Name = "linkedin",
                DisplayName = "LinkedIn",
                IconUrl = "https://www.linkedin.com/favicon.ico",
                Description = "Sign in with your LinkedIn account",
                RequiresScopes = true,
                DefaultScopes = new[] { "r_liteprofile", "r_emailaddress" }
            },
            [Provider.Zoom] = new ProviderInfo
            {
                Name = "zoom",
                DisplayName = "Zoom",
                IconUrl = "https://zoom.us/favicon.ico",
                Description = "Sign in with your Zoom account",
                RequiresScopes = true,
                DefaultScopes = new[] { "user:read" }
            }
        };

        public ProviderService(ILogger<ProviderService> logger)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<ProviderInfo>> GetAvailableProvidersAsync()
        {
            try
            {
                _logger.LogDebug("Getting available OAuth providers");

                var providers = _supportedProviders.Values
                    .Where(p => p.Name != "email" && p.Name != "phone") // Exclude non-OAuth providers
                    .OrderBy(p => p.DisplayName);

                return await Task.FromResult(providers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available providers");
                return Enumerable.Empty<ProviderInfo>();
            }
        }
        public async Task<ProviderInfo?> GetProviderInfoAsync(string providerName)
        {
            try
            {
                var provider = _supportedProviders.Values
                    .FirstOrDefault(p => string.Equals(p.Name, providerName, StringComparison.OrdinalIgnoreCase));

                return await Task.FromResult(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provider info for: {ProviderName}", providerName);
                return null;
            }
        }

        public bool IsProviderSupported(string providerName)
        {
            return _supportedProviders.Values
                .Any(p => string.Equals(p.Name, providerName, StringComparison.OrdinalIgnoreCase));
        }

        public string GetProviderDisplayName(string providerName)
        {
            var provider = _supportedProviders.Values
                .FirstOrDefault(p => string.Equals(p.Name, providerName, StringComparison.OrdinalIgnoreCase));

            return provider?.DisplayName ?? providerName;
        }
    }
}
