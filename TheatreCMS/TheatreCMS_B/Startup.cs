using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheatreCMS_B.Startup))]
namespace TheatreCMS_B
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
