using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Documentation.Web.Startup))]
namespace Documentation.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
