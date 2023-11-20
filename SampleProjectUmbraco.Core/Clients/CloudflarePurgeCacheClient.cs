using SampleProjectUmbraco.Common.Extensions;
using System;
using System.Net.Http;

namespace SampleProjectUmbraco.Core.Clients
{
    /// <summary>
    /// Cloudflare Http client to purge the cloudflare cache
    /// </summary>
    public class CloudflarePurgeCacheClient
    {
        public CloudflarePurgeCacheClient(HttpClient client)
        {
            var clientApiBaseUrl =   UmbracoServicesAccessor.GetSection("Cloudflare:ClientApiBaseUrl");
            var zoneId = UmbracoServicesAccessor.GetSection("Cloudflare:ZoneId");
            var userEmail = UmbracoServicesAccessor.GetSection("Cloudflare:UserEmail");
            var apiKey = UmbracoServicesAccessor.GetSection("Cloudflare:ApiKey");

            client.BaseAddress = new Uri($"{clientApiBaseUrl}/zones/{zoneId}/purge_cache");
            client.DefaultRequestHeaders.Add("X-Auth-Email", userEmail);
            client.DefaultRequestHeaders.Add("X-Auth-Key", apiKey);
        }
    }
}
