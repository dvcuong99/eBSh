using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eBSH.Areas.QT;
using eBSH.Models;
using Identity.Core;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;

namespace eBSH.Areas.manage.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IdentitySignInManager _signInManager;
        private readonly IdentityUserManager _userManager;
        private readonly IdentityRoleManager _roleManager;
        private readonly IAuthenticationManager _authManager;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        //public AccountController()
        //{
        //}

        //public AccountController(IdentityUserManager userManager, IdentitySignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}
        public AccountController(IdentityUserManager userManager, IdentitySignInManager signInManager, IdentityRoleManager roleManager, IAuthenticationManager authenticationManager)
        {
            _authManager = authenticationManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IdentityRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<IdentityRoleManager>();
            }

        }

        public IdentitySignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<IdentitySignInManager>();
            }

        }

        public IdentityUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<IdentityUserManager>();
            }

        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            Log.Error(JsonConvert.SerializeObject(result));
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        public ActionResult Register()
        {
            // Thêm chọn Role khi tạo tài khoản
            List<SelectListItem> listRole = new List<SelectListItem>();
            foreach (var item in RoleManager.Roles)
            {
                listRole.Add(new SelectListItem() { Value = item.Name, Text = item.Name });
                ViewBag.Roles = listRole;
            }
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Admin")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //gán thời gian mở khoá tài khoản khi tạo
                var user = new IdentityUser { UserName = model.Email,UnitID =1, Email = model.Email, LockoutEndDateUtc = DateTimeOffset.Now.AddYears(1000), Audit = new Audit(default(Guid)) };
                Log.Info(JsonConvert.SerializeObject(user));
                //var user = new IdentityUser { UserName = model.Email, Email = model.Email, Audit = new Audit(default(Guid)) };
                var result = await UserManager.CreateAsync(user, model.Password);
                Log.Info(JsonConvert.SerializeObject(result));

                await UserManager.SetLockoutEnabledAsync(user.UserId, false);//Mở khoá tài khoản khi tạo-không comfirm Mail
                if (result.Succeeded)
                {
                    result = await UserManager.AddToRoleAsync(user.UserId, model.RoleName);
                    //Send Mail comfirm
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);--Tắt tự động login khi register

                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Xác nhận tài khoản hệ thống:thecao.bshc.com.vn", "Click vào link để xác nhận tài khoản <a href=\"" + callbackUrl + "\">here</a>");

                    TempData["message"] = "Đăng ký thành công tài khoản:" + user.UserName;
                    return RedirectToAction("ListAccount", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ListAccount(int Page = 1)
        {
            var listUser = UserManager.Users.ToList();
            List<AcountViewModel> data = new List<AcountViewModel>();
            foreach (var item in listUser)
            {
                string roleName = UserManager.GetRoles(item.Id).FirstOrDefault();
                //DateTimeOffset lockDate = UserManager.GetLockoutEndDate(item.Id);
                var lockUser = UserManager.GetLockoutEnabled(item.Id);
                //0 mở 1 đóng
                if (lockUser)
                    data.Add(new AcountViewModel
                    {
                        UserId = item.Id,
                        UserName = item.UserName,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber
                        ,
                        CreateDate = item.CreateDate,
                        RoleName = roleName,
                        TT = 0
                    });
                else
                    data.Add(new AcountViewModel
                    {
                        UserId = item.Id,
                        UserName = item.UserName,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber
                        ,
                        CreateDate = item.CreateDate,
                        RoleName = roleName,
                        TT = 1
                    });
            }
            var insurCardPage = new IUserPaginatedVM
            {
                ItemPerPage = 20,
                IuserList = data,
                CurrentPage = Page
            };
            return View(insurCardPage);

        }

        public ActionResult Edit(string id)
        {
            LoadListEdit();
            AcountViewModel data = new AcountViewModel();
            Guid userId;
            Guid.TryParse(id, out userId);
            try
            {
                var user = UserManager.FindById(userId);
                data.UserName = user.UserName;
                data.UserId = userId;
                data.CreateDate = user.CreateDate;
                data.Email = user.Email;
                data.PhoneNumber = user.PhoneNumber;
                data.RoleName = UserManager.GetRoles(user.Id).FirstOrDefault();
                var lockUser = UserManager.GetLockoutEnabled(data.UserId);
                if (!lockUser)
                    data.TT = 0;
                else
                    data.TT = 1;
                return View(data);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Lỗi hệ thống, hãy thử lại");
                return View(data);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(AcountViewModel model)
        {
            var user = UserManager.FindById(model.UserId);
            if (user != null)
            {
                await UserManager.SetPhoneNumberAsync(model.UserId, model.PhoneNumber);
                await UserManager.SetEmailAsync(model.UserId, model.Email);
                string roleUser = UserManager.GetRoles(user.Id).FirstOrDefault();
                if (roleUser != null)
                    await UserManager.RemoveFromRoleAsync(model.UserId, roleUser);
                await UserManager.AddToRolesAsync(model.UserId, model.RoleName);
                if (model.TT == 0)
                    await UserManager.SetLockoutEnabledAsync(model.UserId, false);
                else
                    await UserManager.SetLockoutEnabledAsync(model.UserId, true);
                TempData["message"] = "Đã cập nhật thông tin tài khoản:" + model.UserName;
                return RedirectToAction("ListAccount");
            }
            else
            {
                return RedirectToAction("Edit");
            }

        }
        [CustomAuthorize(Roles = "Admin")]
        public async Task<ActionResult> LockUser(Guid id)
        {
            try
            {
                var user = UserManager.FindById(id);
                await UserManager.SetLockoutEnabledAsync(id, true);
                //await UserManager.SetLockoutEndDateAsync(id,DateTime.Now.AddYears(200));
                TempData["message"] = "Đã khoá tài khoản:" + user.UserName;
                return RedirectToAction("ListAccount");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Lỗi hệ thống, hãy thử lại");
                return View();
            }
        }
        [CustomAuthorize(Roles = "Admin")]
        public async Task<ActionResult> UnLockUser(Guid id)
        {
            try
            {
                var user = UserManager.FindById(id);
                await UserManager.SetLockoutEnabledAsync(id, false);
                //await UserManager.SetLockoutEndDateAsync(id, DateTime.Now.AddYears(-200));
                TempData["message"] = "Đã mở khoá tài khoản:" + user.UserName;
                return RedirectToAction("ListAccount");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Lỗi hệ thống, hãy thử lại");
                return View();
            }

        }

        public void LoadListEdit()
        {
            List<SelectListItem> listTT = new List<SelectListItem>();
            listTT.Add(new SelectListItem() { Value = "0", Text = "Đang mở" });
            listTT.Add(new SelectListItem() { Value = "1", Text = "Đang đóng" });

            ViewBag.listTT = listTT;
            List<SelectListItem> listRole = new List<SelectListItem>();
            var list = RoleManager.Roles.ToList();
            foreach (var item in list)
            {
                listRole.Add(new SelectListItem() { Value = item.Name, Text = item.Name });
            }
            ViewBag.listRole = listRole;
        }



        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(Guid userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new IdentityUser { UserName = model.Email, Email = model.Email, Audit = new Audit(default(Guid)) };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home", new { area = "manage" });
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    if (UserManager != null)
            //    {
            //        //_userManager.Dispose();
            //        UserManager = null;
            //    }

            //    if (SignInManager != null)
            //    {
            //        //_signInManager.Dispose();
            //        SignInManager = null;
            //    }
            //}

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "insCard2022#B$H";
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return _authManager;
            }
        }
        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }

}