using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(S3WebConsole.Startup))]
namespace S3WebConsole
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
