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
    public class ContentDeletedNotificationHandler : INotificationHandler<ContentDeletedNotification>
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ILogger<ContentDeletedNotificationHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ContentDeletedNotificationHandler(IUmbracoContextAccessor umbracoContextAccessor, ILogger<ContentDeletedNotificationHandler> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public void Handle(ContentDeletedNotification notification)
        {
            try
            {
                if (_configuration.GetValue<bool>("Cloudflare:PurgeEnabled"))
                {
                    Task.Run(() => PurgeCloudflareCacheForDeletedEntities(notification)).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ContentDeletedNotificationHandler exception, ex.Message: {ex.Message}, ex.StackTrace: {ex.StackTrace}");
            }
        }

        private async Task PurgeCloudflareCacheForDeletedEntities(ContentDeletedNotification notification)
        {
            var uRLs = new List<string>();
            IUmbracoContext context;

            foreach (var deletedEntity in notification.DeletedEntities)
            {
                if (_umbracoContextAccessor.TryGetUmbracoContext(out context))
                {
                    var entity = context.Content.GetById(deletedEntity.Id);

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

            _logger.LogInformation($"ContentDeletedNotificationHandler PurgeCloudflareCacheByURLs has been called called for {string.Join(' ', uRLs)}, responseJSON: {response.CloudflareResponseJSON}");
        }
    }
}
