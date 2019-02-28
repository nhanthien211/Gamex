using GamexApi.Identity;
using GamexApiService.Implement;
using GamexApiService.Interface;
using GamexEntity;
using GamexRepository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Data.Entity;
using System.Web;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataHandler.Serializer;
using Microsoft.Owin.Security.DataProtection;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace GamexApi
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            //ASP.NET Identity registration
            container.RegisterType<ApplicationDbContext>();
            container.RegisterType<ApplicationSignInManager>();
            container.RegisterType<ApplicationUserManager>();
            container.RegisterType<ApplicationRoleManager>();
            container.RegisterType<IIdentityMessageService, SendGridEmailService>();

            container.RegisterType<DbContext, ApplicationDbContext>(new ContainerControlledLifetimeManager());
            container.RegisterType<IAuthenticationManager>(
                new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();

            container.RegisterType<IRoleStore<IdentityRole, string>, RoleStore<IdentityRole, string, IdentityUserRole>>();
            ////End of: ASP.NET Identity registration

            //Repo + UoW + DBContext registration
            container.RegisterType<GamexContext>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType(typeof(IRepository<>), typeof(Repository<>));
            //End of :Repo + UoW + DBContext registration

            container.RegisterType<IExhibitionService, ExhibitionService>();

            container.RegisterType(typeof(ISecureDataFormat<>), typeof(SecureDataFormat<>));
            container.RegisterType<ISecureDataFormat<AuthenticationTicket>, SecureDataFormat<AuthenticationTicket>>();
            container.RegisterType<ISecureDataFormat<AuthenticationTicket>, TicketDataFormat>();
            container.RegisterType<IDataSerializer<AuthenticationTicket>, TicketSerializer>();
            container.RegisterType<IDataProtector>(
                new InjectionFactory(c => new DpapiDataProtectionProvider().Create("ASP.NET Identity")));
        }
    }
}