using Statik.Hosting;

namespace Statik.Web.Impl
{
    public class WebBuilderFactory : IWebBuilderFactory
    {
        readonly IHostBuilder _hostBuilder;

        public WebBuilderFactory(IHostBuilder hostBuilder)
        {
            _hostBuilder = hostBuilder;
        }
        
        public IWebBuilder CreateWebBuilder()
        {
            return new WebBuilder(_hostBuilder);
        }
    }
}