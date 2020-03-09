using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace RemotiatR.Shared.Internal
{
    internal class DefaultJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _jsonSerializer;

        public DefaultJsonSerializer(JsonSerializer jsonSerializer) =>
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));

        public object Deserialize(Stream stream, Type type)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (type == null) throw new ArgumentNullException(nameof(type));

            RestartStream(stream);

            using var streamReader = new StreamReader(stream, Encoding.Default);
            using var jsonTextReader = new JsonTextReader(streamReader);

            return _jsonSerializer.Deserialize(jsonTextReader, type)!;
        }

        public Stream Serialize(object value, Type type)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream, Encoding.Default, 1024, true);
            using var jsonTextWriter = new JsonTextWriter(streamWriter);

            jsonTextWriter.CloseOutput = false;
            _jsonSerializer.Serialize(jsonTextWriter, value, type);
            jsonTextWriter.Flush();

            RestartStream(stream);

            return stream;
        }

        private static void RestartStream(Stream stream)
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
