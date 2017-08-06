using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ERMPower.Startup))]
namespace ERMPower
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
