using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CSUploadFileToOneDriveAndShare.Startup))]
namespace CSUploadFileToOneDriveAndShare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
