using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dokan.Web.Startup))]
namespace Dokan.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
