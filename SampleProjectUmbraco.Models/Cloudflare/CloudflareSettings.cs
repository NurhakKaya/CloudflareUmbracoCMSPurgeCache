namespace SampleProjectUmbraco.Models.Cloudflare
{
    public class CloudflareSettings
    {
        public bool PurgeEnabled { get; set; }

        public string ClientApiBaseUrl { get; set; }

        public string ZoneId { get; set; }

        public string UserEmail { get; set; }

        public string ApiKey { get; set; }
    }
}
