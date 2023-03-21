using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBSH.Areas.manage.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: manage/Home
        public ActionResult Index()
        {
            return View(); 
        }
    }
}