// ...removed for brevity

namespace SampleProjectUmbraco.Web
{
    public class Startup
    {
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <remarks>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </remarks>
        public void ConfigureServices(IServiceCollection services)
        {
            // ...removed for brevity
			services.Configure<CloudflareSettings>(_config.GetSection("Cloudflare"));
            services.AddHttpClient("cloudflarePurgeCache", c => new CloudflarePurgeCacheClient(c));

			// ...removed for brevity
        }
		
		// ...removed for brevity
    }
}
