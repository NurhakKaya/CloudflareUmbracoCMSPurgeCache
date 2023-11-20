using Newtonsoft.Json;

namespace SampleProjectUmbraco.Models.Cloudflare
{
    public class CloudflareResponseResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
