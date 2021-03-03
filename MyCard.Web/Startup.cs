using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(MyCard.Web.Startup))]

namespace MyCard.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}