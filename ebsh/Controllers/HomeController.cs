using eBSH.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace eBSH.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index(string refCode)
        {
            //Fetch the Cookie using its Key
            HttpCookie cookie = Request.Cookies["RefCode"];

            if (refCode != null)
            {
                if (cookie == null)
                {
                    //Create a Cookie with a suitable Key
                    cookie = new HttpCookie("RefCode");
                }
                //Set the Cookie value
                cookie.Value = refCode;
                //Set the Expiry date
                cookie.Expires = DateTime.Now.AddDays(15);
                //Add the Cookie to Browser
                Response.Cookies.Add(cookie);
                ViewBag.RefCode = refCode;
            }
            else
            {
                //If Cookie exists fetch its value
                if (cookie != null)
                {
                    ViewBag.RefCode = cookie.Value;
                }
            }
                return View();
        }

        
    }
}
