using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RemotiatR.Server
{
    public class MessageInfoIndex : ReadOnlyDictionary<Uri, MessageInfo>
    {
        public MessageInfoIndex(IDictionary<Uri, MessageInfo> dictionary) : base(dictionary)
        {
        }
    }
}
