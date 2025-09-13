using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupaBaseIntegration.Dtos.Vdocipher;
using SupaBaseIntegration.Services.VdoCipher;

namespace SupaBaseIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVdoCipherService _vdoCipherService;
        private readonly VdoCipherConfig _config;
        private readonly ILogger<VideosController> _logger;

        public VideosController(IVdoCipherService vdoCipherService, ILogger<VideosController> logger, VdoCipherConfig config)
        {
            _vdoCipherService = vdoCipherService;
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<VideoListResponse>> GetVideos(int page = 1, int limit = 20)
        {
            try
            {
                var videos = await _vdoCipherService.GetVideosAsync(page, limit);
                return Ok(videos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving videos");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{videoId}")]
        public async Task<ActionResult<VideoInfo>> GetVideo(string videoId)
        {
            try
            {
                var video = await _vdoCipherService.GetVideoByIdAsync(videoId);
                return Ok(video);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving video {VideoId}", videoId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("otp")]
        public async Task<ActionResult<OtpResponse>> GenerateOtp([FromBody] OtpRequestDto dto)
        {
            try
            {
                var otp = await _vdoCipherService.GenerateOtpAsync(dto);
                return Ok(otp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP for video {VideoId}", dto.VideoId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("upload-credentials")]
        public async Task<ActionResult<UploadCredentials>> GetUploadCredentials([FromBody] VideoUploadRequest request)
        {
            try
            {
                var credentials = await _vdoCipherService.GetUploadCredentialsAsync(request);
                return Ok(credentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upload credentials");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{videoId}")]
        public async Task<ActionResult<VideoInfo>> UpdateVideo(string videoId, [FromBody] VideoUploadRequest request)
        {
            try
            {
                var updatedVideo = await _vdoCipherService.UpdateVideoAsync(videoId, request);
                return Ok(updatedVideo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating video {VideoId}", videoId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{videoId}")]
        public async Task<ActionResult> DeleteVideo(string videoId)
        {
            try
            {
                var success = await _vdoCipherService.DeleteVideoAsync(videoId);
                return success ? Ok() : StatusCode(500, "Failed to delete video");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting video {VideoId}", videoId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("config-check")]
        public ActionResult CheckConfiguration()
        {
            var issues = new List<string>();

            if (string.IsNullOrEmpty(_config.BaseUrl))
                issues.Add("BaseUrl is not configured");
            else if (!_config.BaseUrl.StartsWith("https://"))
                issues.Add("BaseUrl should use HTTPS");

            if (string.IsNullOrEmpty(_config.ApiSecret))
                issues.Add("ApiSecret is not configured");
            else if (_config.ApiSecret.Length < 10)
                issues.Add("ApiSecret seems too short");

            return Ok(new
            {
                isValid = issues.Count == 0,
                issues = issues,
                config = new
                {
                    baseUrl = _config.BaseUrl,
                    hasApiSecret = !string.IsNullOrEmpty(_config.ApiSecret),
                    apiSecretPreview = _config.ApiSecret?.Substring(0, Math.Min(6, _config.ApiSecret?.Length ?? 0)) + "..."
                }
            });
        }
    }
}

