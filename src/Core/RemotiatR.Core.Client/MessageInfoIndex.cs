using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemotiatR.Client
{
    public class MessageInfoIndex : ReadOnlyDictionary<Type, MessageInfo>
    {
        public MessageInfoIndex(IDictionary<Type, MessageInfo> dictionary) : base(dictionary)
        {
        }
    }
}
