using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinalMVC.Startup))]
namespace FinalMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
