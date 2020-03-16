using Newtonsoft.Json;
using RemotiatR.Shared;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RemotiatR.Serializer.Json.Shared
{
    internal class DefaultJsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializer _jsonSerializer;

        public DefaultJsonMessageSerializer()
        {
            _jsonSerializer = new JsonSerializer();
            _jsonSerializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            _jsonSerializer.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
        }

        public Task<object> Deserialize(Stream stream, Type asType)
        {
            try
            {
                if (stream == null) throw new ArgumentNullException(nameof(stream));
                if (asType == null) throw new ArgumentNullException(nameof(asType));

                RestartStream(stream);

                using var streamReader = new StreamReader(stream, Encoding.Default);
                using var jsonTextReader = new JsonTextReader(streamReader);

                var data = _jsonSerializer.Deserialize(jsonTextReader, asType)!;

                return Task.FromResult(data);
            }
            catch(Exception exception)
            {
                throw new Exception("Deserializing failed. See inner exception for details.", exception);
            }
        }

        public Task<Stream> Serialize(object value, Type asType)
        {
            try
            {
                if (asType == null) throw new ArgumentNullException(nameof(asType));

                var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream, Encoding.Default, 1024, true);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);

                jsonTextWriter.CloseOutput = false;
                _jsonSerializer.Serialize(jsonTextWriter, value, asType);
                jsonTextWriter.Flush();

                RestartStream(stream);

                return Task.FromResult((Stream)stream);
            }
            catch (Exception exception)
            {
                throw new Exception("Derializing failed. See inner exception for details.", exception);
            }
        }

        private static void RestartStream(Stream stream)
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        }
    }
}
