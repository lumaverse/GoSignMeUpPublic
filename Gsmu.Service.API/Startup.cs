using Microsoft.Owin;
using Owin;

namespace Gsmu.Service.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}