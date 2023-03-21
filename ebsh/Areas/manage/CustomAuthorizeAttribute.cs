using System;
using System.Web.Mvc;

namespace eBSH.Areas.QT
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string ViewName { get; set; }
        public CustomAuthorizeAttribute()
        {
            ViewName = "AuthorizeFailed";
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            IsUserAuthorized(filterContext);

        }
        public void IsUserAuthorized(AuthorizationContext filterContext)
        {
            if (filterContext.Result == null)
                return;
            ViewDataDictionary dic = new ViewDataDictionary();
            dic.Add("Message", "Tài khoản không có quyền truy cập chức năng này");
            var result = new ViewResult() { ViewName = this.ViewName, ViewData = dic };
            filterContext.Result = result;
        }
    }
}