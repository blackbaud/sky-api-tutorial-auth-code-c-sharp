using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Blackbaud.AuthCodeFlowTutorial
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                name: "Home",
                template: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );   
            routes.MapRoute(
                name: "Authorization",
                template: "auth/{action}",
                defaults: new { action = "authorized" }
            );
        }
    }
}
