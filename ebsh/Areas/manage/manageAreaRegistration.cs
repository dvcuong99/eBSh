using System.Web.Mvc;

namespace eBSH.Areas.manage
{
    public class manageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "manage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "manage_default",
                "manage/{controller}/{action}/{id}",
                new { controller = "home", action = "index", id = UrlParameter.Optional },
                new[] { "eBSH.Areas.manage.Controllers" }
            );
        }
    }
}