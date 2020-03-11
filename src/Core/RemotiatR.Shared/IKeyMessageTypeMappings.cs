using System;
using System.Collections.Generic;

namespace RemotiatR.Shared
{
    public interface IKeyMessageTypeMappings
    {
        IReadOnlyDictionary<string,Type> KeyToMessageTypeLookup { get; }

        IReadOnlyDictionary<Type, string> MessageTypeToKeyLookup { get; }
    }
}
