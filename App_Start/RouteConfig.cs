using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Blackbaud.AuthCodeFlowTutorial
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                name: "Default",
                template: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
