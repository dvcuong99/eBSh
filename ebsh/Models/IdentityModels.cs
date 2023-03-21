
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using eBSH.Models;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using eBSH.Repositories;
using eBSH.Helper;
namespace eBSH.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string sid { get; set; }
        public string ten { get; set; }
        public string phong { get; set; }
        public string cap { get; set; }
        public string ten_dvi { get; set; }
        public string ma_dvi { get; set; }
        public string dchi_dvi { get; set; }
        public string ma_thue { get; set; }
        public string ten_ct { get; set; }
        public string ver { get; set; }
        public string ten_ktt { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            return userIdentity;
        }
    }

    public class BSHUserManager : UserManager<ApplicationUser>
    {
        public BSHUserManager() : base(new UserStore<ApplicationUser>())
        {
        }
        public static BSHUserManager Create(IdentityFactoryOptions<BSHUserManager> options, IOwinContext context)
        {
            var appUserManager = new BSHUserManager();

            return appUserManager;
        }
        //public override Task<ApplicationUser> FindAsync(string userName, string password)
        //{
        //    Task<ApplicationUser> taskInvoke = Task<ApplicationUser>.Factory.StartNew(() =>
        //    {
        //        using (OraDALSession dalSession = new OraDALSession(ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString))
        //        {
        //            UnitOfWork unitOfWork = dalSession.UnitOfWork;
        //            var nsd = new NSDRepo(unitOfWork);
        //            var loginInfo = userName.Split(new string[] { "@" }, System.StringSplitOptions.RemoveEmptyEntries);
        //            var tt = nsd.MaNSD(loginInfo[1].ToUpper(), loginInfo[0].ToUpper(), password);

        //            if (tt != null)
        //            {
        //                ApplicationUser applicationUser = new ApplicationUser();
        //                applicationUser.Id = loginInfo[0].ToUpper();
        //                applicationUser.UserName = loginInfo[0].ToUpper();
        //                applicationUser.ma_dvi = loginInfo[1].ToUpper();
        //                applicationUser.sid = Common.Encrypt(password);
        //                applicationUser.ten = UnicodeConvert.TCVN3ToUnicode(tt.ten);
        //                applicationUser.ten_dvi = UnicodeConvert.TCVN3ToUnicode(tt.ten_dvi);
        //                return applicationUser;
        //            }
        //            return null;
        //        }
        //    });
        //    return taskInvoke;
        //}
        public override Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, string authenticationType)
        {
            Task<ClaimsIdentity> taskInvoke = Task<ClaimsIdentity>.Factory.StartNew(() =>
            {
                var identity = new ClaimsIdentity(
                     new[] { 
                              // adding following 2 claim just for supporting default antiforgery provider
                              new Claim(ClaimTypes.NameIdentifier,  user.UserName),
                              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                              new Claim(ClaimTypes.Name,user.ten),
                              new Claim(ClaimTypes.Locality, user.ten_dvi),
                              // optionally you could add roles if any
                              new Claim(ClaimTypes.GroupSid, user.ma_dvi),
                              new Claim(ClaimTypes.Sid, user.sid)
                            },
                            authenticationType);
                return identity;
            });

            return taskInvoke;
        }
    }

}