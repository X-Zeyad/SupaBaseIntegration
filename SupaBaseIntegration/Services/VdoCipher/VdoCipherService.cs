using SupaBaseIntegration.Dtos.Vdocipher;
using System.Text;
using System.Text.Json;

namespace SupaBaseIntegration.Services.VdoCipher
{
    public class VdoCipherService : IVdoCipherService
    {
        private readonly HttpClient _httpClient;
        private readonly VdoCipherConfig _config;
        private readonly ILogger<VdoCipherService> _logger;

        public VdoCipherService(HttpClient httpClient, VdoCipherConfig config, ILogger<VdoCipherService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;

            // Set base address and default headers
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Apisecret {_config.ApiSecret}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }
        public async Task<VideoListResponse> GetVideosAsync(int page = 1, int limit = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/videos?page={page}&limit={limit}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                return JsonSerializer.Deserialize<VideoListResponse>(content, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching video list");
                throw;
            }
        }

        public async Task<VideoInfo> GetVideoByIdAsync(string videoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/videos/{videoId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var result = JsonSerializer.Deserialize<VideoInfo>(content, options);
                if (result is null)
                {
                    throw new InvalidOperationException($"Deserialization returned null for video ID: {videoId}");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching video with ID: {VideoId}", videoId);
                throw;
            }
        }

        public async Task<OtpResponse> GenerateOtpAsync(OtpRequestDto dto)
        {
            try
            {
                var payload = new { ttl = dto.Ttl };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/videos/{dto.VideoId}/otp", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                return JsonSerializer.Deserialize<OtpResponse>(responseContent, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP for video: {VideoId}", dto.VideoId);
                throw;
            }
        }

        public async Task<UploadCredentials> GetUploadCredentialsAsync(VideoUploadRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("/videos", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                return JsonSerializer.Deserialize<UploadCredentials>(responseContent, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upload credentials");
                throw;
            }
        }

        public async Task<VideoInfo> UpdateVideoAsync(string videoId, VideoUploadRequest updateRequest)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateRequest, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/videos/{videoId}", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var result = JsonSerializer.Deserialize<VideoInfo>(responseContent, options);
                return result is null ?
                    throw new InvalidOperationException($"Deserialization returned null for video ID: {videoId}") : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating video: {VideoId}", videoId);
                throw;
            }
        }

        public async Task<bool> DeleteVideoAsync(string videoId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/videos/{videoId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting video: {VideoId}", videoId);
                return false;
            }
        }
    }
}

