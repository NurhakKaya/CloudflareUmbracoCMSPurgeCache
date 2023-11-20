using Azure;
using SampleProjectUmbraco.Common.Extensions;
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
    public class ContentPublishedNotificationHandler : INotificationHandler<ContentPublishedNotification>
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILogger<ContentPublishedNotificationHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ContentPublishedNotificationHandler(IUmbracoContextAccessor umbracoContextAccessor, ILogger<ContentPublishedNotificationHandler> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public void Handle(ContentPublishedNotification notification)
        {
            try
            {
                if (_configuration.GetValue<bool>("Cloudflare:PurgeEnabled"))
                {
                    Task.Run(() => PurgeCloudflareCacheForPublishedEntities(notification)).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ContentPublishedNotificationHandler exception, ex.Message: {ex.Message}, ex.StackTrace: {ex.StackTrace}");
            }
        }

        private async Task PurgeCloudflareCacheForPublishedEntities(ContentPublishedNotification notification)
        {
            var uRLs = new List<string>();
            IUmbracoContext context;

            foreach (var publishedEntity in notification.PublishedEntities)
            {
                if (_umbracoContextAccessor.TryGetUmbracoContext(out context))
                {
                    var entity = context.Content.GetById(publishedEntity.Id);

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

            var response = await CloudflareCacheHandler.PurgeCloudflareCacheByURLs(uRLs, _httpClientFactory);

            _logger.LogInformation($"ContentPublishedNotificationHandler PurgeCloudflareCacheByURLs has been called called for {string.Join(' ', uRLs)}, responseJSON: {response.CloudflareResponseJSON}");
        }
    }
}
