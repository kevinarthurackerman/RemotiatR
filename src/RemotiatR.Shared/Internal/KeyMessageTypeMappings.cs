using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RemotiatR.Shared.Internal
{
    internal class KeyMessageTypeMappings : IKeyMessageTypeMappings
    {
        public IReadOnlyDictionary<string,Type> KeyToMessageTypeLookup { get; }

        public IReadOnlyDictionary<Type, string> MessageTypeToKeyLookup { get; }

        public KeyMessageTypeMappings(IEnumerable<KeyMessageTypeMapping> keyMessageTypeMappings)
        {
            if (keyMessageTypeMappings == null) throw new ArgumentNullException(nameof(keyMessageTypeMappings));

            var duplicateKey = keyMessageTypeMappings.GroupBy(x => x.Key)
                .Where(x => x.Count() > 1)
                .FirstOrDefault();
            
            if (duplicateKey != null)
                throw new ArgumentException($"Duplicate {nameof(KeyMessageTypeMapping.Key)} {duplicateKey.Key} provided to parameter {nameof(keyMessageTypeMappings)}. {nameof(KeyMessageTypeMapping.Key)} must be unique.");

            var duplicateMessageType = keyMessageTypeMappings.GroupBy(x => x.MessageType)
                .Where(x => x.Count() > 1)
                .FirstOrDefault();

            if (duplicateMessageType != null)
                throw new ArgumentException($"Duplicate {nameof(KeyMessageTypeMapping.MessageType)} {duplicateMessageType.Key} provided to parameter {nameof(keyMessageTypeMappings)}. {nameof(KeyMessageTypeMapping.MessageType)} must be unique.");

            KeyToMessageTypeLookup = new ReadOnlyDictionary<string, Type>(keyMessageTypeMappings.ToDictionary(x => x.Key, x => x.MessageType));
            MessageTypeToKeyLookup = new ReadOnlyDictionary<Type, string>(keyMessageTypeMappings.ToDictionary(x => x.MessageType, x => x.Key));
        }
    }
}
