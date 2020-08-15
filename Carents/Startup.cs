using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Carents.Startup))]
namespace Carents
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
