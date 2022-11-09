using Blackbaud.AuthCodeFlowTutorial.Models;
using Microsoft.Extensions.Options;

namespace Blackbaud.AuthCodeFlowTutorial.Services
{

    /// <summary>
    /// Contains business logic and helper methods that interact with the authentication provider.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {   
        
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ISessionService _sessionService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationService(IOptions<AppSettings> appSettings, ISessionService sessionService, IHttpClientFactory httpClientFactory)
        {
            _appSettings = appSettings;
            _sessionService = sessionService;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Fetches access/refresh tokens from the provider.
        /// <param name="requestBody">Key-value attributes to be sent with the request.</param>
        /// <returns>The response from the provider.</returns>
        /// </summary>
        private async Task<RefreshTokenResponseModel> FetchTokens(Dictionary<string, string> requestBody, CancellationToken cancellationToken) 
        { 
            var client = _httpClientFactory.CreateClient("AuthenticationService");

            // Fetch tokens from auth server.
            var response = await client.PostAsync("token", new FormUrlEncodedContent(requestBody));

            response.EnsureSuccessStatusCode();

            // Parse the response
            var model = await response.Content.ReadFromJsonAsync<RefreshTokenResponseModel>(cancellationToken: cancellationToken);

            // Save the access/refresh tokens in the Session.
            _sessionService.SetTokens(model);
                
            return model;
        }
        
        
        /// <summary>
        /// Fetches a new set of access/refresh tokens (from an authorization code).
        /// <param name="code">The authorization code contained within the provider's authorization response.</param>
        /// </summary>
        public async Task<RefreshTokenResponseModel> ExchangeCodeForAccessToken(string code, string state, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(code, nameof(code));
            ArgumentNullException.ThrowIfNull(state, nameof(state));

            // get the state verifier from session
            var verifier = _sessionService.GetStateVerifier(state);

            // no verifier was found for the state
            if (string.IsNullOrWhiteSpace(verifier))
            {
                throw new Exception("Error verifying authorization code state");
            }

            // get an access token using the authoriztion code
            // the redirect URI must be set up for this client id
            // pass the code verifier
            var response = await FetchTokens(new Dictionary<string, string>(){
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", _appSettings.Value.AuthRedirectUri },
                { "code_verifier", verifier }
            }, cancellationToken);

            // clear the state verifier from the session
            _sessionService.ClearStateVerifier(state);

            // return the response
            return response;
        }
        
        
        /// <summary>
        /// Refreshes the expired access token (from the refresh token stored in the session).
        /// </summary>
        public async Task<RefreshTokenResponseModel> RefreshAccessToken(CancellationToken cancellationToken)
        {
            return await FetchTokens(new Dictionary<string, string>(){
                { "grant_type", "refresh_token" },
                { "refresh_token", _sessionService.GetRefreshToken() }
            }, cancellationToken);
        }
        
        
        /// <summary>
        /// Builds and returns a string representative of the provider's authorization URI.
        /// </summary>
        public Uri GetAuthorizationUri()
        {
            // create a state parameter to identify this request
            var state = Guid.NewGuid().ToString("n");

            // create PKCE values for the authorization
            var pkce = new Pkce();

            // The auth client must have PKCE enabled
            var url = $"authorization?response_type=code&code_challenge_method=S256&client_id={_appSettings.Value.AuthClientId}&redirect_uri={_appSettings.Value.AuthRedirectUri}&code_challenge={pkce.CodeChallenge}&state={state}";

            // store the verifier using the state as the key
            _sessionService.SetStateVerifier(state, pkce.CodeVerifier);

            return new Uri(new Uri(_appSettings.Value.AuthBaseUri), url);
        }
        
        /// <summary>
        /// Determines if the user is authenticated
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
           return IsAccessTokenValid() || HasValidRefreshToken();
        }
        
        /// <summary>
        /// Determines if the session contains a valid access token.
        /// </summary>
        public bool IsAccessTokenValid()
        {
            // get the access token and expires stored in session
            var accessToken = _sessionService.GetAccessToken();
            var expires = _sessionService.GetAccessExpires();

            // if the access token is empty or a minute from expired then it is not valid
            if (
                string.IsNullOrEmpty(accessToken) || 
                !expires.HasValue || 
                (expires.Value.UtcDateTime - DateTimeOffset.UtcNow) <= TimeSpan.FromMinutes(1)
            )
            {
                return false;
            }

            // other cases are valid
            return true;
        }

        /// <summary>
        /// Determine if there is a valid refresh token stored in session
        /// </summary>
        /// <returns></returns>
        public bool HasValidRefreshToken()
        {
            // get the refresh token from session
            var refreshToken = _sessionService.GetRefreshToken();
            var expires = _sessionService.GetRefrehExpires();

            // if the refresh token is empty return false
            if (
                string.IsNullOrEmpty(refreshToken) ||
                !expires.HasValue ||
                (expires.Value.UtcDateTime - DateTimeOffset.UtcNow) <= TimeSpan.FromMinutes(1))
            {
                return false;
            }

            // other cases return true
            return true;
        }

        /// <summary>
        /// Destroys the access/refresh tokens stored in the session.
        /// </summary>
        public void LogOut()
        {
            _sessionService.ClearTokens();
        }
    }
}