using SupaBaseIntegration.Dtos.Vdocipher;
using SupaBaseIntegration.Services.VdoCipher;

namespace SupaBaseIntegration.Extemsions
{
    public static class VdoCipherServiceExtensions
    {
        public static IServiceCollection AddVdoCipher(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new VdoCipherConfig();
            configuration.GetSection("VdoCipher").Bind(config);
            //services.Configure<VdoCipherConfig>(
            //    configuration.GetSection("VdoCipher"));
            services.AddSingleton(config);
            services.AddHttpClient<IVdoCipherService, VdoCipherService>();

            return services;
        }
    }
}
