using SampleProjectUmbraco.Core.BusinessLogic.Cloudflare.NotificationHandlers;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;

namespace SampleProjectUmbraco.Core.BusinessLogic.Cloudflare.Composers
{
    public class CloudflareComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Register Umbraco notification handlers
            builder.AddNotificationHandler<ContentPublishedNotification, ContentPublishedNotificationHandler>(); // Publish
            builder.AddNotificationHandler<ContentUnpublishedNotification, ContentUnpublishedNotificationHandler>(); // Unpublish
            builder.AddNotificationHandler<ContentDeletedNotification, ContentDeletedNotificationHandler>(); // Deleted
        }
    }
}
