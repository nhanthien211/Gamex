using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GamexWeb.Startup))]
namespace GamexWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
