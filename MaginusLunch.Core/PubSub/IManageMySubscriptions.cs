using System;

namespace MaginusLunch.Core.PubSub
{
    public interface IManageMySubscriptions
    {
        void SubscribeTo(Type anEventType);
        void UnsubscribeTo(Type anEventType);
    }
}
