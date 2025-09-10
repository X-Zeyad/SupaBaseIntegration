using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupaBaseIntegration.Dtos;
using SupaBaseIntegration.Services;

namespace SupaBaseIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IProviderService _providerService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IProviderService providerService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _providerService = providerService;
            _logger = logger;
        }
        #region Basic Authentication

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SignUpAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                message = "User created successfully"
            });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SignInAsync(dto.Email, dto.Password);

            if (!result.Success)
                return Unauthorized(new { message = result.ErrorMessage });

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                message = "Sign in successful"
            });
        }

        [HttpPost("signout")]
        public async Task<IActionResult> SignOut()
        {
            var token = GetTokenFromHeader();
            var result = await _authService.SignOutAsync(token);

            if (result)
                return Ok(new { message = "Signed out successfully" });
            else
                return BadRequest(new { message = "Sign out failed" });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "No authorization token provided" });

            var user = await _authService.GetCurrentUserAsync(token);

            if (user == null)
                return Unauthorized(new { message = "Invalid or expired token" });

            return Ok(new { user, message = "Current user retrieved successfully" });
        }

        #endregion

        #region Magic Link & OTP Authentication
        [HttpPost("signin/magiclink")]
        public async Task<IActionResult> SignInWithMagicLink([FromBody] MagicLinkDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) && string.IsNullOrEmpty(dto.Phone))
                return BadRequest(new { message = "Either email or phone number is required" });

            if (!string.IsNullOrEmpty(dto.Email))
            {
                var result = await _authService.SendMagicLinkAsync(dto.Email, dto.RedirectTo);

                if (!result.Success)
                    return BadRequest(new { message = result.ErrorMessage });

                return Ok(new
                {
                    message = "Magic link sent successfully",
                    sentTo = result.SentTo,
                    type = "email"
                });
            }
            else
            {
                var result = await _authService.SendOtpAsync(dto.Phone!);

                if (!result.Success)
                    return BadRequest(new { message = result.ErrorMessage });

                return Ok(new
                {
                    message = "OTP sent successfully",
                    sentTo = result.SentTo,
                    type = "phone"
                });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPDto dto)
        {
            if (string.IsNullOrEmpty(dto.Token))
                return BadRequest(new { message = "OTP token is required" });

            if (string.IsNullOrEmpty(dto.Email) && string.IsNullOrEmpty(dto.Phone))
                return BadRequest(new { message = "Either email or phone number is required" });

            var result = await _authService.VerifyOtpAsync(dto.Token, dto.Email, dto.Phone);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new
            {
                user = result.User,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                message = "OTP verification successful"
            });
        }

        #endregion
        #region OAuth Providers
        [HttpGet("providers")]
        public async Task<IActionResult> GetProviders()
        {
            var providers = await _providerService.GetAvailableProvidersAsync();
            return Ok(providers);
        }
        [HttpGet("providers/{providerName}")]
        public async Task<IActionResult> GetProviderInfo(string providerName)
        {
            var provider = await _providerService.GetProviderInfoAsync(providerName);
            if (provider == null)
                return NotFound(new { message = "Provider not found" });
            return Ok(provider);
        }
        [HttpGet("oauth-url")]
        public async Task<IActionResult> GetOAuthUrl([FromQuery] string provider, [FromQuery] string? redirectTo = null)
        {
            var result = await _authService.GetOAuthUrlAsync(provider, new OAuthOptions
            {
                RedirectTo = redirectTo
            });

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
        [HttpPost("oauth-callback")]
        public async Task<IActionResult> HandleOAuthCallback([FromBody] OAuthCallbackRequest request)
        {
            var result = await _authService.HandleOAuthCallbackAsync(request.AccessToken, request.RefreshToken);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        #endregion

        private string? GetTokenFromHeader()
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }
        public record SignInDto(string Email, string Password);
        public record MagicLinkDto(string? Email = null, string? Phone = null, string? RedirectTo = null);
        public record VerifyOTPDto(string Token, string? Email = null, string? Phone = null);
        public record OAuthCallbackRequest(string AccessToken, string? RefreshToken = null);

    }
}
