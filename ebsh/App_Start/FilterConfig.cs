using System.Web;
using System.Web.Mvc;

namespace eBSH
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
#if DEBUG
            if (!HttpContext.Current.IsDebuggingEnabled)
                filters.Add(new RequireHttpsAttribute());
#else
            filters.Add(new RequireHttpsAttribute());
#endif  
        }

    }
}
