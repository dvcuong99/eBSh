using System.Web.Mvc;
using System.Web.Routing;

namespace eBSH
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "home", action = "index", id = UrlParameter.Optional },
                //new { controller = "cyber", action = "regorder", id = UrlParameter.Optional },
                //new { controller = "topcare", action = "regorder", id = UrlParameter.Optional },
                new[] { "eBSH.Controllers" }
            );
        }
    }
}
