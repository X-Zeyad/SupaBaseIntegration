using SupaBaseIntegration.Dtos;

namespace SupaBaseIntegration.Services
{
    public interface IProviderService
    {
        Task<IEnumerable<ProviderInfo>> GetAvailableProvidersAsync();
        Task<ProviderInfo?> GetProviderInfoAsync(string providerName);
        bool IsProviderSupported(string providerName);
        string GetProviderDisplayName(string providerName);
    }
}
