using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BidMVC.Startup))]
namespace BidMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
