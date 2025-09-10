using Supabase;
using Supabase.Gotrue;
using SupaBaseIntegration.Dtos;
using static Supabase.Gotrue.Constants;
using Client = Supabase.Client;

namespace SupaBaseIntegration.Services
{
    public class AuthService : IAuthService
    {
        private readonly Client _supabase;
        private readonly ILogger<AuthService> _logger;

        public AuthService(Client supabase, ILogger<AuthService> logger)
        {
            _supabase = supabase;
            _logger = logger;
        }

        #region Basic Authentication

        public async Task<AuthResult> SignUpAsync(SignUpDto dto)
        {
            try
            {
                _logger.LogInformation("Attempting to sign up user with email: {Email}", dto.Email);
                var signUpOptions = new Supabase.Gotrue.SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "display_name", dto.FullName },
                        { "phone", dto.Phone }
                    }

                };
                var response = await _supabase.Auth.SignUp(dto.Email, dto.Password , signUpOptions);

                if (response?.User == null)
                {
                    _logger.LogWarning("Sign up failed for email: {Email}", dto.Email);
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Sign up failed. Please check your credentials."
                    };
                }

                _logger.LogInformation("User signed up successfully: {UserId}", response.User.Id);

                return new AuthResult
                {
                    Success = true,
                    User = response.User,
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sign up for email: {Email}", dto.Email);
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = $"Sign up failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResult> SignInAsync(string email, string password)
        {
            try
            {
                _logger.LogInformation("Attempting to sign in user with email: {Email}", email);

                var response = await _supabase.Auth.SignIn(email, password);

                if (response?.User == null)
                {
                    _logger.LogWarning("Sign in failed for email: {Email}", email);
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid credentials. Please check your email and password."
                    };
                }

                _logger.LogInformation("User signed in successfully: {UserId}", response.User.Id);

                return new AuthResult
                {
                    Success = true,
                    User = response.User,
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sign in for email: {Email}", email);
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = $"Sign in failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> SignOutAsync(string? token = null, string? refreshToken = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token) || !string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogInformation("Signing out user with provided token");
                    await _supabase.Auth.SetSession(token, refreshToken);
                }
                else
                {
                    _logger.LogInformation("Signing out current user");
                }
                await _supabase.Auth.SignOut();
                _logger.LogInformation("User signed out successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sign out");
                return false;
            }
        }

        public async Task<string?> GetCurrentUserAsync(string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    // Try to get user info directly from token instead of setting session
                    var response = await _supabase.Auth.GetUser(token);
                    if (response != null)
                    {
                        _logger.LogDebug("Current user retrieved: {UserId}", response.Email);
                        return response.Email;
                    }
                }

                var user = _supabase.Auth.CurrentUser;

                if (user != null)
                {
                    _logger.LogDebug("Current user retrieved: {UserId}", user.Id);
                }

                return user.Email;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return null;
            }
        }

        #endregion

        #region Magic Link & OTP

        public async Task<MagicLinkResult> SendMagicLinkAsync(string email, string? redirectTo = null)
        {
            try
            {
                _logger.LogInformation("Sending magic link to email: {Email}", email);

                var options = new SignInOptions();
                if (!string.IsNullOrEmpty(redirectTo))
                {
                    options.RedirectTo = redirectTo;
                }

                var result = await _supabase.Auth.SendMagicLink(email, options);

                if (result)
                {
                    _logger.LogInformation("Magic link sent successfully to: {Email}", email);
                    return new MagicLinkResult
                    {
                        Success = true,
                        SentTo = email
                    };
                }
                else
                {
                    _logger.LogWarning("Failed to send magic link to: {Email}", email);
                    return new MagicLinkResult
                    {
                        Success = false,
                        ErrorMessage = "Failed to send magic link"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending magic link to: {Email}", email);
                return new MagicLinkResult
                {
                    Success = false,
                    ErrorMessage = $"Magic link failed: {ex.Message}"
                };
            }
        }

        public async Task<OtpResult> SendOtpAsync(string phone)
        {
            try
            {
                _logger.LogInformation("Sending OTP to phone: {Phone}", MaskPhone(phone));

                // Trigger OTP sending via phone
                await _supabase.Auth.SignIn(SignInType.Phone, phone);

                _logger.LogInformation("OTP sent successfully to: {Phone}", MaskPhone(phone));
                return new OtpResult
                {
                    Success = true,
                    SentTo = phone
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to: {Phone}", MaskPhone(phone));
                return new OtpResult
                {
                    Success = false,
                    ErrorMessage = $"OTP failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResult> VerifyOtpAsync(string token, string? email = null, string? phone = null)
        {
            try
            {
                if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone))
                {
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Either email or phone number is required for OTP verification"
                    };
                }

                _logger.LogInformation("Verifying OTP for: {Identifier}", email ?? MaskPhone(phone!));

                Session response;

                if (!string.IsNullOrEmpty(email))
                {
                    response = await _supabase.Auth.VerifyOTP(email, token, EmailOtpType.MagicLink);
                }
                else
                {
                    response = await _supabase.Auth.VerifyOTP(phone, token, MobileOtpType.SMS);
                }

                if (response?.User != null)
                {
                    _logger.LogInformation("OTP verified successfully for user: {UserId}", response.User.Id);
                    return new AuthResult
                    {
                        Success = true,
                        User = response.User,
                        AccessToken = response.AccessToken,
                        RefreshToken = response.RefreshToken
                    };
                }
                else
                {
                    _logger.LogWarning("OTP verification failed for: {Identifier}", email ?? MaskPhone(phone!));
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Invalid or expired OTP"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for: {Identifier}", email ?? MaskPhone(phone!));
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = $"OTP verification failed: {ex.Message}"
                };
            }
        }

        #endregion
        #region OAuth

        public async Task<OAuthUrlResult> GetOAuthUrlAsync(string provider, OAuthOptions? options = null)
        {
            try
            {
                if (!Enum.TryParse<Provider>(provider, true, out var oauthProvider))
                {
                    return new OAuthUrlResult
                    {
                        Success = false,
                        ErrorMessage = $"Unsupported OAuth provider: {provider}"
                    };
                }

                _logger.LogInformation("Generating OAuth URL for provider: {Provider}", provider);

                var signInOptions = new SignInOptions();

                if (options != null)
                {
                    if (!string.IsNullOrEmpty(options.RedirectTo))
                        signInOptions.RedirectTo = options.RedirectTo;

                    if (options.Scopes?.Any() == true)
                        signInOptions.Scopes = string.Join(" ", options.Scopes);

                    if (options.QueryParams?.Any() == true)
                        signInOptions.QueryParams = options.QueryParams;
                }

                var signInUrl = await Task.Run(() => _supabase.Auth.SignIn(oauthProvider, signInOptions));

                _logger.LogInformation("OAuth URL generated successfully for provider: {Provider}", provider);

                return new OAuthUrlResult
                {
                    Success = true,
                    Provider = provider
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OAuth URL for provider: {Provider}", provider);
                return new OAuthUrlResult
                {
                    Success = false,
                    ErrorMessage = $"OAuth URL generation failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResult> HandleOAuthCallbackAsync(string accessToken, string? refreshToken = null)
        {
            try
            {
                _logger.LogInformation("Handling OAuth callback");

                var response = await _supabase.Auth.SetSession(accessToken, refreshToken);

                if (response?.User != null)
                {
                    _logger.LogInformation("OAuth callback handled successfully for user: {UserId}", response.User.Id);
                    return new AuthResult
                    {
                        Success = true,
                        User = response.User,
                        AccessToken = response.AccessToken,
                        RefreshToken = response.RefreshToken
                    };
                }
                else
                {
                    _logger.LogWarning("OAuth callback failed - invalid tokens");
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Failed to authenticate with OAuth tokens"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling OAuth callback");
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = $"OAuth callback failed: {ex.Message}"
                };
            }
        }

        #endregion

        #region Session Management

        public async Task<bool> RefreshSessionAsync(string refreshToken)
        {
            try
            {
                _logger.LogInformation("Refreshing session");

                var response = await _supabase.Auth.SetSession(null, refreshToken);

                if (response?.User != null)
                {
                    _logger.LogInformation("Session refreshed successfully for user: {UserId}", response.User.Id);
                    return true;
                }
                _logger.LogWarning("Session refresh failed - invalid refresh token");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing session");
                return false;
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                var user = await GetUserFromTokenAsync(token);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return false;
            }
        }

        public async Task<User?> GetUserFromTokenAsync(string token)
        {
            try
            {
                await _supabase.Auth.SetSession(token, null);
                return _supabase.Auth.CurrentUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user from token");
                return null;
            }
        }

        #endregion

        #region Helper Methods

        private static string MaskPhone(string phone)
        {
            if (phone.Length <= 4) return phone;
            return $"{phone[..2]}***{phone[^2..]}";
        }

        #endregion
    }
}
