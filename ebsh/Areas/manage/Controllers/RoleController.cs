using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Identity.Core;
using eBSH.Models;
using Microsoft.AspNet.Identity.Owin;
using eBSH.Areas.QT;
using System.Web.Security;

namespace eBSH.Areas.manage.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private IdentityRoleManager _roleManage;
        public RoleController() { }

        // GET: QT/Default
        public RoleController(IdentityRoleManager roleManage)
        {
            RoleManage = roleManage;
        }
        public IdentityRoleManager RoleManage
        {
            get
            {
                return _roleManage ?? HttpContext.GetOwinContext().GetUserManager<IdentityRoleManager>();
            }
            private set
            {
                _roleManage = value;
            }
        }
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<RoleViewModel> list = new List<RoleViewModel>();
            foreach (var item in RoleManage.Roles)
            {
                list.Add(new RoleViewModel(item));
            }
            return View(list);
        }
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel model)
        {
            var role = new IdentityRole() { Name = model.Name };
            await RoleManage.CreateAsync(role);
            return RedirectToAction("Index");
        }
    }
}