using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CSLoginOnOffice365AndGetAccountInfo.Startup))]
namespace CSLoginOnOffice365AndGetAccountInfo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
