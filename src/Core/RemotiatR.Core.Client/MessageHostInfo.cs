using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RemotiatR.Shared;

namespace RemotiatR.Client
{
    internal class MEssageHostInfo : IMessageHostInfo
    {
        internal static IMessageHostInfo NoopMessageHostInfo = new MEssageHostInfo();

        public Uri RootUri { get; }
        public Type MessageSerializerType { get; }
        public Type MessageTransportType { get; }
        public IReadOnlyDictionary<Type, IMessageInfo> MessageInfos { get; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private MEssageHostInfo() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        internal MEssageHostInfo(Uri rootUri, Type messageSerializerType, Type messageTransportType, IEnumerable<IMessageInfo> messageInfos)
        {
            RootUri = rootUri ?? throw new ArgumentNullException(nameof(rootUri));
            MessageSerializerType = messageSerializerType ?? throw new ArgumentNullException(nameof(messageSerializerType));
            if (!typeof(IMessageSerializer).IsAssignableFrom(messageSerializerType))
                throw new ArgumentException($"{nameof(messageSerializerType)} {messageSerializerType.FullName} is not assignable to {typeof(IMessageSerializer).FullName}");
            MessageTransportType = messageTransportType ?? throw new ArgumentNullException(nameof(messageTransportType));
            if (!typeof(IMessageTransport).IsAssignableFrom(messageTransportType))
                throw new ArgumentException($"{nameof(messageTransportType)} {messageTransportType.FullName} is not assignable to {typeof(IMessageTransport).FullName}");
            if (messageInfos == null) throw new ArgumentNullException(nameof(messageInfos));
            MessageInfos = new ReadOnlyDictionary<Type, IMessageInfo>(messageInfos.ToDictionary(x => x.MessageType));
        }
    }
}
