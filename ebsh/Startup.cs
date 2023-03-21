using Autofac;
using Autofac.Integration.Mvc;
using eBSH.Core;
using eBSH.Repositories;
using Identity.Core;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using System;
using System.Web;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(eBSH.Startup))]
namespace eBSH
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // REGISTER DEPENDENCIES
            builder.RegisterType<ConnectionFactory>().As<IConnectionFactory>();
            builder.RegisterType<DbSession>().As<IDbSession>();

            builder.RegisterType<UserStore<IdentityUser>>().As<IUserStore<IdentityUser, Guid>>().InstancePerRequest();
            builder.RegisterType<RoleStore<IdentityRole>>().As<IRoleStore<IdentityRole, Guid>>().InstancePerRequest();
            builder.RegisterType<IdentityUserManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<IdentitySignInManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<IdentityRoleManager>().AsSelf().InstancePerRequest();

            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.Register<IDataProtectionProvider>(c => app.GetDataProtectionProvider()).InstancePerRequest();

           

            builder.RegisterType<GCNRepository>().As<IGCNRepository>();
            builder.RegisterType<GiftCodeRepository>().As<IGiftCodeRepository>();
            builder.RegisterType<TopCarePremiumRepo>().As<ITopCarePremiumRepo>();
            builder.RegisterType<OrderRepository>().As<IOrderRepository>(); 
            builder.RegisterType<PA_GCNRepository>().As<IPA_GCNRepository>();
            builder.RegisterType<MotorcycleRepository>().As<IMotorcycleRepository>();
            builder.RegisterType<MotorVehicleRepository>().As<IMotorVehicleRepository>();
            builder.RegisterType<RefCodeRepository>().As<IRefCodeRepository>();
            // REGISTER CONTROLLERSpe< SO DEPENDENCIES ARE CONSTRUCTOR INJECTED
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // BUILD THE CONTAINER
            var container = builder.Build();

            // REPLACE THE MVC DEPENDENCY RESOLVER WITH AUTOFAC
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // REGISTER WITH OWIN
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();

            ConfigureAuth(app);
        }

    }

}