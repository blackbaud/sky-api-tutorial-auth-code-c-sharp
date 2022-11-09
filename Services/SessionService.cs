using Blackbaud.AuthCodeFlowTutorial.Models;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{

    /// <summary>
    /// Sets, gets, and destroys session variables.
    /// </summary>
    public class SessionService : ISessionService
    {
        private const string ACCESS_TOKEN_NAME = "token";
        private const string REFRESH_TOKEN_NAME = "refreshToken";
        private const string ACCESS_TOKEN_EXPIRES = "access_expires";
        private const string REFRESH_TOKEN_EXPIRES = "refresh_expires";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        /// <summary>
        /// Destroys access and refresh tokens from the session.
        /// </summary>
        public void ClearTokens()
        {
            try
            {
                _httpContextAccessor.HttpContext.Session.Remove(ACCESS_TOKEN_NAME);
                _httpContextAccessor.HttpContext.Session.Remove(REFRESH_TOKEN_NAME);
                _httpContextAccessor.HttpContext.Session.Remove(ACCESS_TOKEN_EXPIRES);
                _httpContextAccessor.HttpContext.Session.Remove(REFRESH_TOKEN_EXPIRES);
            }
            catch (Exception error)
            {
                Console.WriteLine("LOGOUT ERROR: " + error.Message);
            }
        }


        /// <summary>
        /// Return access token, if saved, or an empty string.
        /// </summary>
        public string GetAccessToken()
        {
            if (TryGetString(ACCESS_TOKEN_NAME, out var accessToken))
            {
                return accessToken;
            }
            return null!;
        }


        /// <summary>
        /// Return refresh token, if saved, or an empty string.
        /// </summary>
        public string GetRefreshToken()
        {
            if (TryGetString(REFRESH_TOKEN_NAME, out var refreshToken))
            {
                return refreshToken;
            }
            return null!;
        }

        /// <summary>
        /// Get the access token expiration date
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset? GetAccessExpires()
        {
            if (TryGetString(ACCESS_TOKEN_EXPIRES, out var expires))
            {
                if (DateTimeOffset.TryParse(expires, out var expirationDate))
                {
                    return expirationDate;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the refresh token expiration date
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset? GetRefrehExpires()
        {
            if (TryGetString(REFRESH_TOKEN_EXPIRES, out var expires))
            {
                if (DateTimeOffset.TryParse(expires, out var expirationDate))
                {
                    return expirationDate;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the code verifier based on the state
        /// </summary>
        /// <param name="state">The state value of the authorization request</param>
        /// <returns></returns>
        public string GetStateVerifier(string state)
        {
            if (TryGetString(state, out var verifier))
            {
                return verifier;
            }
            return null!;
        }

        /// <summary>
        /// Store the code verifier in session by the state value
        /// </summary>
        /// <param name="state">The state value</param>
        /// <param name="verifier">The code verifier</param>
        public void SetStateVerifier(string state, string verifier)
        {
            _httpContextAccessor.HttpContext.Session.SetString(state, verifier);
        }

        /// <summary>
        /// Clear the state verifier
        /// </summary>
        /// <param name="state">The state</param>
        public void ClearStateVerifier(string state)
        {
            _httpContextAccessor.HttpContext.Session.Remove(state);
        }

        /// <summary>
        /// Sets the access and refresh tokens based on an HTTP response.
        /// </summary>
        public void SetTokens(RefreshTokenResponseModel response)
        {
            _httpContextAccessor.HttpContext.Session.SetString(ACCESS_TOKEN_NAME, response.AccessToken);
            _httpContextAccessor.HttpContext.Session.SetString(ACCESS_TOKEN_EXPIRES, $"{DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn)}");
            _httpContextAccessor.HttpContext.Session.SetString(REFRESH_TOKEN_NAME, response.RefreshToken);
            _httpContextAccessor.HttpContext.Session.SetString(REFRESH_TOKEN_EXPIRES, $"{DateTimeOffset.UtcNow.AddSeconds(response.RefreshTokenExpiresIn.GetValueOrDefault())}");
        }


        /// <summary>
        /// Return session value as a string (if it exists), or an empty string.
        /// </summary>
        private bool TryGetString(string name, out string value)
        {
            value = null!;
            if (_httpContextAccessor.HttpContext.Session.TryGetValue(name, out var valueBytes))
            {
                value = System.Text.Encoding.UTF8.GetString(valueBytes);
                return true;
            }
            return false;
        }
    }
}