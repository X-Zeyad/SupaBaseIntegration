//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Supabase;
//using Supabase.Gotrue;
//using Client = Supabase.Client;

//namespace SupaBaseIntegration.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class Auth2Controller : ControllerBase
//    {
//        private readonly Client _supabase;

//        public Auth2Controller(Client supabase)
//        {
//            _supabase = supabase;
//        }
//        [HttpPost("signup")]
//        public async Task<IActionResult> SignUp([FromBody] SignUpDto dto)
//        {
//            try
//            {
//                var response = await _supabase.Auth.SignUp(dto.Email, dto.Password);
//                if (response?.User == null)
//                    return BadRequest("Signup failed");

//                return Ok(response.User);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest($"Signup failed: {ex.Message}");
//            }
//        }
//        [HttpPost("signin")]
//        public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
//        {
//            try
//            {
//                var response = await _supabase.Auth.SignIn(dto.Email, dto.Password);

//                if (response?.User == null)
//                    return Unauthorized("Invalid credentials");

//                return Ok(new
//                {
//                    User = response.User,
//                    AccessToken = response.AccessToken,
//                    RefreshToken = response.RefreshToken
//                });
//            }
//            catch (Exception ex)
//            {
//                return BadRequest($"Sign in failed: {ex.Message}");
//            }
//        }
//        [HttpPost("signout")]
//        public async Task<IActionResult> SignOut()
//        {
//            try
//            {
//                var token = GetTokenFromHeader();
//                if (!string.IsNullOrEmpty(token))
//                {
//                    await _supabase.Auth.SetSession(token, null);
//                }

//                await _supabase.Auth.SignOut();
//                return Ok("Signed out");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest($"Sign out failed: {ex.Message}");
//            }
//        }
//        [HttpGet("me")]
//        public async Task<IActionResult> GetCurrentUserAsync()
//        {
//            try
//            {
//                var token = GetTokenFromHeader();
//                if (string.IsNullOrEmpty(token))
//                    return Unauthorized("No token provided");

//                await _supabase.Auth.SetSession(token, null);

//                var user = _supabase.Auth.CurrentUser;
//                return user != null ? Ok(user) : Unauthorized("Invalid token");
//            }
//            catch (Exception ex)
//            {
//                return Unauthorized($"Authentication failed: {ex.Message}");
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> SendPasswordResetEmail(string email)
//        {
//            var attrs = new UserAttributes { Email = "new-email@example.com" };
//            var response = await _supabase.Auth.Update(attrs);
//            if (response.Error != null)
//                return BadRequest(response.Error.Message);
//            return Ok("Password reset email sent");
//        }
//        private string? GetTokenFromHeader()
//        {
//            var authHeader = Request.Headers.Authorization.FirstOrDefault();
//            if (authHeader != null && authHeader.StartsWith("Bearer "))
//            {
//                return authHeader.Substring("Bearer ".Length).Trim();
//            }
//            return null;
//        }


//    }
    
   
//    public record OAuthOptionsDto(string? RedirectTo = null, List<string>? Scopes = null, Dictionary<string, string>? QueryParams = null);
//    
    
//}
