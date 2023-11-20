using Newtonsoft.Json;
using System.Collections.Generic;

namespace SampleProjectUmbraco.Models.Cloudflare
{
    public class CloudflareFileInfo
    {
        [JsonProperty("files")]
        public List<string> Files { get; set; }
    }
}
