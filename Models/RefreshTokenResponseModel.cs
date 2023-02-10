using System.Text.Json.Serialization;

namespace Blackbaud.AuthCodeFlowTutorial.Models
{
    public class RefreshTokenResponseModel
    {
        /// <summary>
        /// Access Token
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Token Type
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Expires In
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Environment ID
        /// </summary>
        [JsonPropertyName("environment_id")]
        public string EnvironmentId { get; set; }

        /// <summary>
        /// Environment Name
        /// </summary>
        [JsonPropertyName("environment_name")]
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Legal Entity Id
        /// </summary>
        [JsonPropertyName("legal_entity_id")]
        public string LegalEntityId { get; set; }

        /// <summary>
        /// Legal Entity Name
        /// </summary>
        [JsonPropertyName("legal_entity_name")]
        public string LegalEntityName { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// Family Name
        /// </summary>
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        /// Given Name
        /// </summary>
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        /// <summary>
        /// Refresh Token Expires In
        /// </summary>
        [JsonPropertyName("refresh_token_expires_in")]
        public int? RefreshTokenExpiresIn { get; set; }

        /// <summary>
        /// The access mode (Full, Limited)
        /// </summary>
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        /// <summary>
        /// The scope of the response (space delimited)
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }
}