namespace Blackbaud.AuthCodeFlowTutorial.Services
{
    
    /// <summary>
    /// Stores app-wide configuration properties, mapped to appsettings.json.
    /// </summary>
    public class AppSettings
    {
        public string AuthBaseUri { get; set; }
        public string AuthClientId { get; set; }
        public string AuthClientSecret { get; set; }
        public string AuthRedirectUri { get; set; }
        public string AuthSubscriptionKey { get; set; }
        public string SkyApiBaseUri { get; set; }
    }
}
