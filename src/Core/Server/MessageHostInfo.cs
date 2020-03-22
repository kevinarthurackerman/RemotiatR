using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RemotiatR.Shared;

namespace RemotiatR.Server
{
    internal class MessageHostInfo : IMessageHostInfo
    {
        internal static IMessageHostInfo NoopMessageHostInfo = new MessageHostInfo();

        public Uri RootUri { get; }
        public Type MessageSerializerType { get; }
        public IReadOnlyDictionary<Type, IMessageInfo> MessageInfos { get; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        private MessageHostInfo() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        internal MessageHostInfo(Uri rootUri, Type messageSerializerType, IEnumerable<IMessageInfo> messageInfos)
        {
            RootUri = rootUri ?? throw new ArgumentNullException(nameof(rootUri));
            MessageSerializerType = messageSerializerType ?? throw new ArgumentNullException(nameof(messageSerializerType));
            if (!typeof(IMessageSerializer).IsAssignableFrom(messageSerializerType))
                throw new ArgumentException($"{nameof(messageSerializerType)} {messageSerializerType.FullName} is not assignable to {typeof(IMessageSerializer).FullName}");
            if (messageInfos == null) throw new ArgumentNullException(nameof(messageInfos));
            MessageInfos = new ReadOnlyDictionary<Type,IMessageInfo>(messageInfos.ToDictionary(x => x.MessageType));
        }
    }
}
