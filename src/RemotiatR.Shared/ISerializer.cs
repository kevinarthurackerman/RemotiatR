using System;
using System.IO;

namespace RemotiatR.Shared
{
    public interface ISerializer
    {
        object Deserialize(Stream stream, Type type);

        Stream Serialize(object value, Type type);
    }
}
