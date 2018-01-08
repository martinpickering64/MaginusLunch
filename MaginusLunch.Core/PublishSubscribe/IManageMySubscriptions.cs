using System;

namespace MaginusLunch.Core.PublishSubscribe
{
    public interface IManageMySubscriptions
    {
        void SubscribeTo(Type anEventType);
        void UnsubscribeTo(Type anEventType);
    }
}
