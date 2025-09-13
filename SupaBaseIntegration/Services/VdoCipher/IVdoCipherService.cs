using SupaBaseIntegration.Dtos.Vdocipher;

namespace SupaBaseIntegration.Services.VdoCipher
{
    public interface IVdoCipherService
    {
        Task<VideoListResponse> GetVideosAsync(int page = 1, int limit = 20);
        Task<VideoInfo> GetVideoByIdAsync(string videoId);
        Task<OtpResponse> GenerateOtpAsync(OtpRequestDto dto);
        Task<UploadCredentials> GetUploadCredentialsAsync(VideoUploadRequest request);
        Task<VideoInfo> UpdateVideoAsync(string videoId, VideoUploadRequest updateRequest);
        Task<bool> DeleteVideoAsync(string videoId);
    }
}
