using SampleProjectUmbraco.Core.BusinessLogic.Cloudflare.CacheHandlers;
using SampleProjectUmbraco.Core.BusinessLogic.Cloudflare.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Web;

namespace SampleProjectUmbraco.Core.BusinessLogic.Cloudflare.NotificationHandlers
{
    public class ContentUnpublishedNotificationHandler : INotificationHandler<ContentUnpublishedNotification>
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILogger<ContentUnpublishedNotificationHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ContentUnpublishedNotificationHandler(IUmbracoContextAccessor umbracoContextAccessor, ILogger<ContentUnpublishedNotificationHandler> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public void Handle(ContentUnpublishedNotification notification)
        {
            try
            {
                if (_configuration.GetValue<bool>("Cloudflare:PurgeEnabled"))
                {
                    Task.Run(() => PurgeCloudflareCacheForUnpublishedEntities(notification)).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ContentUnpublishedNotificationHandler exception, ex.Message: {ex.Message}, ex.StackTrace: {ex.StackTrace}");
            }
        }

        private async Task PurgeCloudflareCacheForUnpublishedEntities(ContentUnpublishedNotification notification)
        {
            var uRLs = new List<string>();
            IUmbracoContext context;

            foreach (var unpublishedEntity in notification.UnpublishedEntities)
            {

                if (_umbracoContextAccessor.TryGetUmbracoContext(out context))
                {
                    var entity = context.Content.GetById(unpublishedEntity.Id);

                    if (entity != null)
                    {
                        var entityUrl = Umbraco.Extensions.FriendlyPublishedContentExtensions.Url(entity);

                        if (!string.IsNullOrEmpty(entityUrl))
                        {
                            uRLs.Add(CloudflareNotificationHelper.GetEntityUrl(entityUrl));
                        }
                    }
                }
            }

            var response = await CloudflareCacheHandler.PurgeCloudflareCacheByURLs(uRLs,_httpClientFactory);

            _logger.LogInformation($"ContentUnpublishedNotificationHandler PurgeCloudflareCacheByURLs has been called called for {string.Join(' ', uRLs)}, responseJSON: {response.CloudflareResponseJSON}");
        }
    }
}
