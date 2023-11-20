using SampleProjectUmbraco.Models.Cloudflare;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SampleProjectUmbraco.Core.BusinessLogic.Cloudflare.CacheHandlers
{
    public static class CloudflareCacheHandler
    {
        /// <summary>
        /// Purges cloudflare cache by URLs
        /// </summary>
        /// <param name="uRLs"></param>
        /// <param name="httpClientFactory"></param>
        /// <returns></returns>
        public static async Task<CloudflarePurgeCacheResponse> PurgeCloudflareCacheByURLs(List<string> uRLs, IHttpClientFactory httpClientFactory)
        {
            CloudflarePurgeCacheResponse purgeResponse = null;

            if (uRLs != null && uRLs.Count > 0)
            {
                var serializedData = JsonConvert.SerializeObject(new CloudflareFileInfo
                {
                    Files = uRLs
                });

                purgeResponse = await Purge(httpClientFactory, serializedData);
            }

            return purgeResponse;
        }

        /// <summary>
        /// Purges entire website's cloudflare cache
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <returns></returns>
        public static async Task<CloudflarePurgeCacheResponse> PurgeEntireWebsite(IHttpClientFactory httpClientFactory)
        {
            var serializedData = JsonConvert.SerializeObject(@"{""purge_everything"":true}");

            return await Purge(httpClientFactory, serializedData);
        }

        /// <summary>
        /// Purges cloudflare cache using the serializedData
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="serializedData"></param>
        /// <returns></returns>
        private static async Task<CloudflarePurgeCacheResponse> Purge(IHttpClientFactory httpClientFactory, string serializedData)
        {
            CloudflarePurgeCacheResponse purgeResponse = null;

            var client = httpClientFactory.CreateClient("cloudflarePurgeCache");

            var requestContent = new StringContent(serializedData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(client.BaseAddress, requestContent);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(responseBody))
            {
                purgeResponse = new CloudflarePurgeCacheResponse()
                {
                    CloudflareResponse = JsonConvert.DeserializeObject<CloudflareResponse>(responseBody),
                    CloudflareResponseJSON = responseBody
                };
            }

            return purgeResponse;
        }
    }
}
