using System;
using RemotiatR.Shared;

namespace RemotiatR.Server
{
    internal class MessageInfo : IMessageInfo
    {
        public Type MessageType { get; }
        public Uri EndpointPath { get; }
        public MediatorTypes MediatorType { get; }
        public IMessageHostInfo HostInfo { get; private set; } = MessageHostInfo.NoopMessageHostInfo;

        public MessageInfo(Type messageType, Uri endpointPath)
        {
            MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
            EndpointPath = endpointPath ?? throw new ArgumentNullException(nameof(endpointPath));
            if (endpointPath.IsAbsoluteUri) throw new InvalidOperationException($"{nameof(endpointPath)} must be relative");

            if (MessageType.IsRequestType())
            {
                if (MessageType.IsNotificationType())
                    throw new ArgumentException($"{nameof(messageType)} {messageType.FullName} must not be both a notification and a request type");

                MediatorType = MediatorTypes.Request;
            }
            else if(MessageType.IsNotificationType())
            {
                MediatorType = MediatorTypes.Notification;
            }
            else
            {
                throw new ArgumentException($"{nameof(messageType)} {messageType.FullName} is neither a notification or a request type");
            }
        }

        internal void SetHost(IMessageHostInfo messageHostInfo) =>
            HostInfo = messageHostInfo ?? throw new ArgumentNullException(nameof(messageHostInfo));
    }
}
