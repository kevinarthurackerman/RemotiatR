using System;

namespace RemotiatR.Shared
{
    public class KeyMessageTypeMapping
    {
        public string Key { get; }
        public Type MessageType { get; }

        public KeyMessageTypeMapping(string key, Type messageType)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
        }
    }
}
