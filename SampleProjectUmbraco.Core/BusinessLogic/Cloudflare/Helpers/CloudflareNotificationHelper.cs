using SampleProjectUmbraco.Common.Extensions;

namespace SampleProjectUmbraco.Core.BusinessLogic.Cloudflare.Helpers
{
    public static class CloudflareNotificationHelper
    {
        public static string GetEntityUrl(string entityUrl)
        {
            return $"https://{UmbracoServicesAccessor.GetSection("Cloudflare:DomainUrl")}{entityUrl}";
        }
    }
}
