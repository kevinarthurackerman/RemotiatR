using System;

namespace RemotiatR.Shared
{
    public class MessageInfo
    {
        public Uri Path { get; }
        public Type RequestType { get; }
        public Type? ResponseType { get; }
        public MessageTypes Type { get; }

        public MessageInfo(Uri path, Type requestType)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            if (path.IsAbsoluteUri) throw new ArgumentException("Must be relative path", nameof(path));
            RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));

            if (requestType.IsNotificationType())
            {
                if (requestType.IsRequestType())
                    throw new InvalidOperationException($"Type of {nameof(requestType)} {requestType.FullName} must not be both a request and a notification");

                Type = MessageTypes.Notification;
            }
            else if(requestType.IsRequestType())
            {
                Type = MessageTypes.Request;
                ResponseType = requestType.GetResponseType();
            }
            else
            {
                throw new InvalidOperationException($"Type of {nameof(requestType)} {requestType.FullName} must either be a request or notification, but not both");
            }
        }
    }

    public enum MessageTypes
    {
        Notification,
        Request
    }
}
