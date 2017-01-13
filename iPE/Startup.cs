using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(iPE.Startup))]
namespace iPE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
