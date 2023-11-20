using Newtonsoft.Json;
using System.Collections.Generic;

namespace SampleProjectUmbraco.Models.Cloudflare
{
    public class CloudflareResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("errors")]
        public IList<object> Errors { get; set; }

        [JsonProperty("messages")]
        public IList<object> Messages { get; set; }

        [JsonProperty("result")]
        public CloudflareResponseResult Result { get; set; }
    }
}
