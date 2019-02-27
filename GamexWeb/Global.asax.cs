using System.Data.Entity.SqlServer;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GamexWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SqlProviderServices.SqlServerTypesAssemblyName =
                "Microsoft.SqlServer.Types, Version=13.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
            GlobalFilters.Filters.Add(new RequireHttpsAttribute());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
