using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace RemotiatR.Shared
{
    public class DefaultJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _jsonSerializer;

        public DefaultJsonSerializer(JsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public object Deserialize(Stream stream, Type type)
        {
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            return _jsonSerializer.Deserialize(jsonTextReader, type);
        }

        public Stream Serialize(object value, Type type)
        {
            var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream, Encoding.Default, 1024, true);
            using var jsonTextWriter = new JsonTextWriter(streamWriter);
            jsonTextWriter.CloseOutput = false;
            _jsonSerializer.Serialize(jsonTextWriter, value, type);
            jsonTextWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
